using AsyncAwaitBestPractices;
using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.Data.JsonModels;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Interfaces.ServiceInterfaces;
using AuthorVerseServer.Models;
using AuthorVerseServer.Models.ContentModels;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace AuthorVerseServer.Services.SectionCreateManager
{
    public class SectionCreateManager : ISectionCreateManager
    {
        private readonly ICreator _creator;
        private readonly IDatabase _redis;
        private readonly LoadFileService _loadFile;
        private readonly ILogger<SectionCreateManager> _logger;

        public SectionCreateManager(IConnectionMultiplexer connectionMultiplexer,
            LoadFileService loadFile, ILogger<SectionCreateManager> logger, ICreator creator)
        {
            _creator = creator;
            _loadFile = loadFile;
            _logger = logger;
            _creator = creator;
            _redis = connectionMultiplexer.GetDatabase();
        }

        public async ValueTask<ICollection<string>?> CreateManagerAsync(string userId, int chapterId)
        {
            var manager = await _redis.SortedSetRangeByRankWithScoresAsync($"manager:{userId}");
            if (manager.Length == 0)
            {
                _redis.StringSetAsync($"managerInfo:{userId}", chapterId, flags: CommandFlags.FireAndForget)
                    .SafeFireAndForget();
                return null;
            }

            // отправить пользователю всю информацию об его прошлых изменениях до выхода и повторного входа
            ICollection<string> collection = new List<string>();
            foreach (var content in manager)
            {
                var contentValue = await _redis.StringGetAsync($"content:{userId}:{content.Element}");
                collection.Add(contentValue!);
            }


            var hashEntries = await _redis.HashGetAllAsync($"choiceManager:{userId}");
            
            foreach (var choice in hashEntries)
            {
                collection.Add(choice.ToString());
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

            // часть кода для сохранения контента секций 
            var manager = await _redis.SortedSetRangeByRankWithScoresAsync($"manager:{userId}");
            foreach (var content in manager)
            {
                int number = (int)content.Score;
                int flow = int.Parse(content.Element.ToString().Split(":")[1]);

                var contentValue = await _redis.StringGetAsync($"content:{userId}:{number}:{flow}");

                var contentDto = JsonConvert.DeserializeObject<ContentBaseJm>(contentValue!,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All })!;

                if (contentDto.Operation == ChangeType.Create)
                {
                    await CreateContentAsync(contentDto, chapterId, number, flow);
                } 
                else if (contentDto.Operation == ChangeType.Update)
                {
                    await UpdateContentAsync(contentDto, chapterId, number, flow);
                } 
                else if (contentDto.Operation == ChangeType.Delete)
                {
                    await DeleteContentAsync(contentDto, chapterId, number, flow);
                 
                    // удалить выборы после удалении самой секции в redis
                    // DeleteChoiceAfterSectionDelete(userId, number, flow);
                }
                else 
                {
                    throw new Exception("You still have not did it");
                }
            }
            
            // часть кода для сохранение данных о выборах в секциях и разветвлениях сюжета 
            var choiceManager = await _redis.HashGetAllAsync($"choiceManager:{userId}");
            foreach (var redisChoice in choiceManager)
            {
                var choice = JsonConvert.DeserializeObject<ChoiceContent>(redisChoice.Value!)!;
                if (choice.Operation == ChangeType.Create)
                {
                    choice.SetChoiceKey = redisChoice.Name!;
                    await CreateChoiceAsync(choice, chapterId, 
                        choice.GetChoiceNumber(), choice.GetNumber(), choice.GetFlow());
                } 
                else if (choice.Operation == ChangeType.Update)
                {
                    // await UpdateContentAsync(choice, chapterId, number, flow);
                } 
                else if (choice.Operation == ChangeType.Delete)
                {
                    // await DeleteContentAsync(choice, chapterId, number, flow);
                }
                else 
                {
                    throw new Exception("You still have not did it");
                }
            }

            ClearRedisAsync(manager, userId).SafeFireAndForget(Print);

            return await _creator.SaveAsync();
        }

        private async Task ClearRedisAsync(SortedSetEntry[] manager, string userId)
        {
            foreach (var contentSection in manager)
            {
                int number = (int)contentSection.Score;
                await _redis.KeyDeleteAsync(
                    $"content:{userId}:{number}:{int.Parse(contentSection.Element.ToString().Split(":")[1])}",
                    flags: CommandFlags.FireAndForget);
            }

            await _redis.KeyDeleteAsync($"manager:{userId}");
            await _redis.KeyDeleteAsync($"choiceManager:{userId}");
        }

        private ValueTask<EntityEntry> CreateContentAsync<T>(T contentDto, int chapterId, int number, int flow) where T : ContentBaseJm
        {
            var model = contentDto.CreateModel();

            if (contentDto is IFileContent fileContent)
            {
                _loadFile.CreateFileAsync(fileContent.SectionContent, fileContent.GetUrl(), fileContent.GetPath())
                    .SafeFireAndForget(Print);
            }

            var section = new ChapterSection()
            {
                BookChapterId = chapterId,
                ChoiceFlow = flow,
                Number = number,
                ContentType = contentDto.GetContentType(),
                ContentBase = model,
            };

            return _creator.AddContentAsync(section);
        }

        private async Task UpdateContentAsync<T>(T contentDto, int chapterId, int number, int flow) where T : ContentBaseJm
        {
            var model = contentDto.CreateModel();

            var dbSection = await _creator.GetSectionAsync(chapterId, number, flow);

            if (contentDto is IFileContent fileContent)
            {
                _loadFile.CreateFileAsync(fileContent.SectionContent, fileContent.GetUrl(), fileContent.GetPath())
                    .SafeFireAndForget(Print);

                _loadFile.DeleteFile(fileContent.GetPath(), fileContent.GetUrl());
            }

            dbSection.ContentType = contentDto.GetContentType();

            if (contentDto is TextContentJm textContent && dbSection.ContentBase is TextContent textModel)
            {
                textModel.Text = textContent.SectionContent;
            }
            else
            {
                _creator.DeleteContent(dbSection.ContentBase);
                dbSection.ContentBase = model;
            }
        }

        private async Task DeleteContentAsync<T>(T contentDto, int chapterId, int number, int flow) where T : ContentBaseJm
        {
            var dbSection = await _creator.GetSectionAsync(chapterId, number, flow);

            if (contentDto is IFileContent fileContent)
            {
                _loadFile.DeleteFile(fileContent.GetPath(), fileContent.GetUrl());
            }

            _creator.DeleteContent(dbSection.ContentBase);

            _creator.DeleteSection(dbSection);
        }

        private void Print(Exception ex)
        {
            _logger.LogError($"Ошибка при сохранение данных. Ошибка: {ex}");
        }

        private async Task CreateChoiceAsync(ChoiceContent choiceContent, int chapterId, 
            int choiceNumber, int number, int flow)
        {
            var choice = new SectionChoice()
            {
                ChoiceNumber = choiceNumber,
                ChapterId = chapterId,
                Number = number,
                Flow = flow,
                ChoiceText = choiceContent.Content,
                TargetChapterId = choiceContent.NextChapterId,
                TargetNumber = choiceContent.NextNumber,
                TargetFlow = choiceContent.NextFlow,
            };
            
            var isExist = await _creator.CheckExistNextSectionAsync(chapterId, number, flow);
            if (!isExist)
            {
                throw new Exception("The next section for the choice does not exist");
            }
            
            await _creator.AddContentAsync(choice);
        }

        // private void DeleteChoiceAfterSectionDelete(string userId, int choiceNumber, int number, int flow)
        // {
        //     _redis.HashDeleteAsync($"choiceManager:{userId}", $"{choiceNumber}:{number}:{flow}", CommandFlags.FireAndForget);
        // }
    }
}
