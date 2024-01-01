using AuthorVerseServer.Data.JsonModels;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Interfaces.ServiceInterfaces;
using AuthorVerseServer.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using AsyncAwaitBestPractices;
using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.Models.ContentModels;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json.Linq;
using System;

namespace AuthorVerseServer.Services
{
    public class SectionCreateManager : ISectionCreateManager
    {
        private readonly IChapterSection _section;
        private readonly IDatabase _redis;
        private readonly LoadFileService _loadFile;
        private readonly ILogger<LoadFileService> _logger;

        public SectionCreateManager(IChapterSection section, IConnectionMultiplexer connectionMultiplexer, LoadFileService loadFile, ILogger<LoadFileService> logger)
        {
            _section = section;
            _loadFile = loadFile;
            _logger = logger;
            _redis = connectionMultiplexer.GetDatabase();
        }


        public async ValueTask<ICollection<string>?> CreateManagerAsync(string userId, int chapterId)
        {
            var manager = await _redis.SortedSetRangeByRankWithScoresAsync($"manager:{userId}");
            if (manager.Length == 0)
            {
                _redis.StringSetAsync($"managerInfo:{userId}", chapterId, flags: CommandFlags.FireAndForget).SafeFireAndForget();
                return null;
            }

            // отправить пользователю всю информацию об его прошлых изменениях до выхода и повторного входа
            ICollection<string> collection = new List<string>();
            foreach (var content in manager)
            {
                var contentValue = await _redis.StringGetAsync($"content:{userId}:{content.Element}");
                collection.Add(contentValue!);
            }

            return collection;
        }

        public async Task<int> ManagerSaveAsync(string userId)
        {
            var managerInfo = await _redis.StringGetAsync($"managerInfo:{userId}");
            if (!int.TryParse(managerInfo, out int chapterId))
            {
                throw new Exception("The creating session has time out");
            }

            var manager = await _redis.SortedSetRangeByRankWithScoresAsync($"manager:{userId}");
            foreach (var content in manager)
            {
                int number = (int)content.Score;
                int flow = int.Parse(content.Element.ToString().Split(":")[1]);

                var contentValue = await _redis.StringGetAsync($"content:{userId}:{number}:{flow}");

                var contentDTO = JsonConvert.DeserializeObject<ContentBaseJM>(contentValue!, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All })!;

                if (contentDTO.Operation == ChangeType.Create)
                {
                    await CreateContentAsync(contentDTO, chapterId, number, flow);
                } 
                else if (contentDTO.Operation == ChangeType.Update)
                {
                    await UpdateContentAsync(contentDTO, chapterId, number, flow);
                } 
                else if (contentDTO.Operation == ChangeType.Delete)
                {
                    await DeleteContentAsync(contentDTO, chapterId, number, flow);
                }
                else 
                {
                    throw new Exception("You still have not did it");
                }
            }

            ClearRedisAsync(manager, userId).SafeFireAndForget(Print, false);

            return await _section.SaveAsync();
        }

        public async Task ClearRedisAsync(SortedSetEntry[] manager, string userId)
        {
            //var manager = await _redis.SortedSetRangeByRankWithScoresAsync($"manager:{userId}");

            foreach (var contentSection in manager)
            {
                int number = (int)contentSection.Score;
                await _redis.KeyDeleteAsync($"content:{userId}:{number}:{int.Parse(contentSection.Element.ToString().Split(":")[1])}", flags: CommandFlags.FireAndForget);
            }

            await _redis.KeyDeleteAsync($"manager:{userId}");
        }

        private ValueTask<EntityEntry> CreateContentAsync<T>(T contentDTO, int chapterId, int number, int flow) where T : ContentBaseJM
        {
            var model = contentDTO!.CreateModel();

            if (contentDTO is IFileContent fileContent)
            {
                _loadFile.CreateFileAsync(fileContent.SectionContent, fileContent.Url, fileContent.Path).SafeFireAndForget(Print, false);
            }

            var section = new ChapterSection()
            {
                BookChapterId = chapterId,
                ChoiceFlow = flow,
                Number = number,
                ContentType = contentDTO.Type,
                ContentBase = model,
            };

            return _section.AddContentAsync(section);
        }

        private async Task UpdateContentAsync<T>(T contentDTO, int chapterId, int number, int flow) where T : ContentBaseJM
        {
            var model = contentDTO!.CreateModel();

            var dbSection = await _section.GetSectionAsync(chapterId, number, flow);

            if (contentDTO is IFileContent fileContent)
            {
                _loadFile.CreateFileAsync(fileContent.SectionContent, fileContent.Url, fileContent.Path).SafeFireAndForget(Print, false);

                //var dbContent = await UseContentType.GetContent(_section, dbSection.ContentType).Invoke(dbSection.ContentId);
                _loadFile.DeleteFile(fileContent.Path, fileContent.Url);
            }

            dbSection.ContentType = contentDTO.Type;

            if (contentDTO is TextContentJM textContent && dbSection.ContentBase is TextContent textModel)
            {
                textModel.Text = textContent.SectionContent;
            }
            else
            {
                _section.DeleteContent(dbSection.ContentBase);
                dbSection.ContentBase = model;
            }
        }

        private async Task DeleteContentAsync<T>(T contentDTO, int chapterId, int number, int flow) where T : ContentBaseJM
        {
            var dbSection = await _section.GetSectionAsync(chapterId, number, flow);

            if (contentDTO is IFileContent fileContent)
            {
                _loadFile.DeleteFile(fileContent.Path, fileContent.Url);
            }

            _section.DeleteContent(dbSection.ContentBase);

            _section.DeleteSection(dbSection);
        }


        private void Print(Exception ex)
        {
            _logger.LogError($"Ошибка при сохранение данных. Ошибка: {ex}");
        }

        public async ValueTask<string> DeleteSectionAsync(string userId, int number, int flow)
        {
            var managerInfo = await _redis.StringGetAsync($"managerInfo:{userId}");
            if (!int.TryParse(managerInfo, out int chapterId))
            {
                return "The creating session has time out";
            }

            var existNextKey = await _redis.KeyExistsAsync($"content:{userId}:{number+1}:{flow}");

            if (existNextKey)
            {
                return "This is no the last section";
            }

            var content = await _redis.StringGetAsync($"content:{userId}:{number}:{flow}");

            if (content.IsNullOrEmpty)
            {
                // если в редисе ничего не содержится, то проверить в бд есть ли такой элемент и добавить в редис контент на удаление
                if (await _section.CheckAddingNewSectionAsync(chapterId, flow) != number)
                {
                    return "The section cannot be deleted from the db";
                }

                // добавить контент на удаление
                var contentBase = new ContentBaseJM()
                {
                    Operation = ChangeType.Delete,
                };

                var jsonContent = JsonConvert.SerializeObject(contentBase);

                _redis.SortedSetAddAsync($"manager:{userId}", $"{number}:{flow}", number).SafeFireAndForget();
                _redis.StringSetAsync($"content:{userId}:{number}:{flow}", jsonContent, flags: CommandFlags.FireAndForget).SafeFireAndForget();

            } else {
                // если в редисе что-то содержится, значит в бд ничего нет, и можно спокойно удалить данные в редис и всё
                _redis.KeyDeleteAsync($"content:{userId}:{number}:{flow}", flags: CommandFlags.FireAndForget).SafeFireAndForget();
                _redis.SortedSetRemoveAsync($"manager:{userId}", $"{number}:{flow}", flags: CommandFlags.FireAndForget).SafeFireAndForget();
            }


            return string.Empty;
        }

        private async Task<ChangeType> CheckContentAsync(string userId, int number, int flow)
        {
            // достаём номер главы
            var managerInfo = await _redis.StringGetAsync($"managerInfo:{userId}");
            if (!int.TryParse(managerInfo, out int chapterId))
            {
                throw new Exception("The creating session has time out");
            }

            // есть ли предыдущий элемент
            var checkBeforeAsync = await _redis.StringGetAsync($"content:{userId}:{number - 1}:{flow}");

            if (checkBeforeAsync.HasValue) 
            {
                var operationType = JObject.Parse(checkBeforeAsync!)["operation"]!.ToObject<ChangeType>();
                if (operationType == ChangeType.Delete)
                {
                    throw new Exception("The last section was deleted, and you can't add the next values");
                }
            } 

            // если ли уже текущий элемент и если это удаление, то гуд, а если нет, то ошибка
            var checkContent = await _redis.StringGetAsync($"content:{userId}:{number}:{flow}");

            ChangeType changeType = ChangeType.Create;
            if (checkContent.HasValue)
            {
                if (JObject.Parse(checkContent!)["operation"]!.ToObject<ChangeType>() == ChangeType.Delete)
                {
                    // скорее всего это удаление, иначе ошибка
                    changeType = ChangeType.Update;
                }
                else
                {
                    throw new Exception("The section with this number and in this flow already exists");
                }
            }
            else if (checkBeforeAsync.IsNullOrEmpty && await _section.CheckAddingNewSectionAsync(chapterId, flow) != number - 1)
            {
                throw new Exception("The section cannot be added to the db");
            }

            return changeType;
        }

        public ValueTask SetValueToRedisAsync(string userId, int number, int flow, string value)
        {
            _redis.StringSetAsync($"content:{userId}:{number}:{flow}", value, TimeSpan.FromHours(3), flags: CommandFlags.FireAndForget);
            _redis.SortedSetAddAsync($"manager:{userId}", $"{number}:{flow}", number, flags: CommandFlags.FireAndForget);
            return ValueTask.CompletedTask;
        }

        public async ValueTask<string> CreateTextSectionAsync(string userId, int number, int flow, string text)
        {
            var changeType = await CheckContentAsync(userId, number, flow);

            var content = new TextContentJM()
            {
                SectionContent = text,
                Operation = changeType,
            };

            string value = JsonConvert.SerializeObject(content, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

            SetValueToRedisAsync(userId, number, flow, value).SafeFireAndForget(Print);

            return string.Empty;
        }

        public async ValueTask<string> CreateImageSectionAsync(string userId, int number, int flow, IFormFile file)
        {
            var changeType = await CheckContentAsync(userId, number, flow);

            var content = new ImageContentJM()
            {
                SectionContent = GetBytesFromIFormFile(file),
                Expansion = Path.GetExtension(file.FileName),
                Operation = changeType,
            };

            string value = JsonConvert.SerializeObject(content, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

            SetValueToRedisAsync(userId, number, flow, value).SafeFireAndForget(Print);

            return string.Empty;
        }

        public ValueTask<string> UpdateTextSectionAsync(string userId, int number, int flow, string text)
        {
            throw new NotImplementedException();
        }


        byte[] GetBytesFromIFormFile(IFormFile file)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);

                byte[] byteArray = memoryStream.ToArray();

                return byteArray;
            }
        }

    }
}
