using AuthorVerseServer.Data.JsonModels;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Interfaces.ServiceInterfaces;
using AuthorVerseServer.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using AuthorVerseServer.Models.ContentModels;

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
            // здесь потому будет множество
            var manager = await _redis.SortedSetRangeByRankWithScoresAsync($"manager:{userId}");
            if (manager.Length == 0)
            {
                _redis.StringSetAsync($"managerInfo:{userId}", chapterId, TimeSpan.FromMinutes(1), flags: CommandFlags.FireAndForget);
                return null;
            }
            else
            {
                // отправить пользователю всю информацию об его прошлых изменениях до выхода и повторного входа
                ICollection<string> collection = new List<string>();
                foreach (var content in manager)
                {
                    var contentValue = await _redis.StringGetAsync($"content:{userId}:{content.Score}:{content.Element}");
                    collection.Add(contentValue!);
                }

                return collection;
            }
        }

        public async Task<string> ManagerSaveAsync(string userId)
        {
            var managerInfo = await _redis.StringGetAsync($"managerInfo:{userId}");
            if (!int.TryParse(managerInfo, out int chapterId))
            {
                return "The creating session has time out";
            }

            var manager = await _redis.SortedSetRangeByRankWithScoresAsync($"manager:{userId}");
            foreach (var content in manager)
            {
                int number = (int)content.Score;
                int flow = int.Parse(content.Element.ToString().Split(":")[0]);

                var contentValue = await _redis.StringGetAsync($"content:{userId}:{number}:{flow}");

                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                var contentDTO = JsonConvert.DeserializeObject<ContentBaseJM>(contentValue!, settings);

                var model = contentDTO.CreateModel();

                if (contentDTO is IFileContent fileContent)
                {
                    await _loadFile.CreateFileAsync(fileContent.SectionContent, fileContent.Url, fileContent.Path);
                }

                var section = new ChapterSection()
                {
                    BookChapterId = chapterId,
                    ChoiceFlow = flow,
                    Number = number,
                    ContentType = contentDTO.Type,
                    ContentBase = model,
                };

                await _section.AddContentAsync(section);
                //await contentDTO.AddContext(_section, model);
            }

            await _section.SaveAsync();

            return string.Empty;
        }

        public TextContent CreateModel(TextContentJM textContent)
        {
            var model = new TextContent()
            {
                Text = textContent.SectionContent,
            };

            return model;
        }

        private void Print(Exception ex)
        {
            _logger.LogError($"Ошибка при сохранение данных. Ошибка: {ex}");
        }


        public async ValueTask<string> DeleteSectionAsync(string userId, int number, int flow)
        {
            //var managerExist = await _redis.KeyExistsAsync($"manager:{userId}");
            //if (managerExist == false)
            //{
            //    return "The creating session has time out";
            //}

            var content = await _redis.StringGetAsync($"content:{userId}:{number}:{flow}");

            if (content.IsNullOrEmpty)
            {
                return "The section with this number and in this flow doesn't exist";
            }

            _redis.SortedSetRemoveAsync($"manager:{userId}", content, flags: CommandFlags.FireAndForget);
            _redis.KeyDeleteAsync($"content:{userId}:{number}:{flow}", flags: CommandFlags.FireAndForget);

            return string.Empty;
        }

        public async ValueTask<string> CreateTextSectionAsync(string userId, int number, int flow, string text)
        {
            var managerInfo = await _redis.StringGetAsync($"managerInfo:{userId}");
            if (!int.TryParse(managerInfo, out int chapterId))
            {
                return "The creating session has time out";
            }

            var checkContent = await _redis.KeyExistsAsync($"content:{userId}:{number}:{flow}");

            if (checkContent == true)
            {
                return "The section with this number and in this flow already exists";
            }

            var checkBeforeAsync = await _redis.KeyExistsAsync($"content:{userId}:{number-1}:{flow}");

            if (checkBeforeAsync == false)
            {
                if (await _section.CheckAddingNewSectionAsync(chapterId, flow) != number - 1)
                {
                    return "The section cannot be added to the db";
                }
            }

            var content = new TextContentJM()
            {
                SectionContent = text,
                Operation = Data.Enums.ChangeType.Create,
            };

            string value = JsonConvert.SerializeObject(content);

            _redis.StringSetAsync($"content:{userId}:{number}:{flow}", value, TimeSpan.FromHours(3), flags: CommandFlags.FireAndForget);
            _redis.SortedSetAddAsync($"manager:{userId}",flow, number, flags: CommandFlags.FireAndForget);

            return string.Empty;
        }

        public async ValueTask<string> CreateImageSectionAsync(string userId, int number, int flow, IFormFile file)
        {
            var managerInfo = await _redis.StringGetAsync($"managerInfo:{userId}");
            if (!int.TryParse(managerInfo, out int chapterId))
            {
                return "The creating session has time out";
            }

            var checkContent = await _redis.KeyExistsAsync($"content:{userId}:{number}:{flow}");

            if (checkContent == true)
            {
                return "The section with this number and in this flow already exists";
            }

            var checkBeforeAsync = await _redis.KeyExistsAsync($"content:{userId}:{number - 1}:{flow}");

            if (checkBeforeAsync == false)
            {
                if (await _section.CheckAddingNewSectionAsync(chapterId, flow) != number - 1)
                {
                    return "The section cannot be added to the db";
                }
            }

            var content = new ImageContentJM()
            {
                SectionContent = GetBytesFromIFormFile(file),
                Expansion = Path.GetExtension(file.FileName),
                Operation = Data.Enums.ChangeType.Create,
            };

            string value = JsonConvert.SerializeObject(content);

            _redis.StringSetAsync($"content:{userId}:{number}:{flow}", value, TimeSpan.FromHours(3), flags: CommandFlags.FireAndForget);
            _redis.SortedSetAddAsync($"manager:{userId}", flow, number, flags: CommandFlags.FireAndForget);

            return string.Empty;
        }

        byte[] GetBytesFromIFormFile(IFormFile file)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Копирование содержимого файла в MemoryStream
                file.CopyTo(memoryStream);

                // Получение массива байтов из MemoryStream
                byte[] byteArray = memoryStream.ToArray();

                return byteArray;
            }
        }

    }
}
