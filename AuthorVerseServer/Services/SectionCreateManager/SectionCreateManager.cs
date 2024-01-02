using AuthorVerseServer.Data.JsonModels;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Interfaces.ServiceInterfaces;
using AuthorVerseServer.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using AsyncAwaitBestPractices;
using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.Interfaces.ServiceInterfaces.SectionCreateManager;
using AuthorVerseServer.Models.ContentModels;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json.Linq;

namespace AuthorVerseServer.Services
{
    public class SectionCreateManager : ISectionCreateManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IChapterSection _section;
        private readonly IDatabase _redis;
        private readonly LoadFileService _loadFile;
        private readonly ILogger<SectionCreateManager> _logger;

        public SectionCreateManager(IServiceProvider serviceProvider, IChapterSection section, 
            IConnectionMultiplexer connectionMultiplexer,
            LoadFileService loadFile, ILogger<SectionCreateManager> logger)
        {
            _serviceProvider = serviceProvider;
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

        private async Task ClearRedisAsync(SortedSetEntry[] manager, string userId)
        {
            //var manager = await _redis.SortedSetRangeByRankWithScoresAsync($"manager:{userId}");

            foreach (var contentSection in manager)
            {
                int number = (int)contentSection.Score;
                await _redis.KeyDeleteAsync(
                    $"content:{userId}:{number}:{int.Parse(contentSection.Element.ToString().Split(":")[1])}",
                    flags: CommandFlags.FireAndForget);
            }

            await _redis.KeyDeleteAsync($"manager:{userId}");
        }

        private ValueTask<EntityEntry> CreateContentAsync<T>(T contentDTO, int chapterId, int number, int flow) where T : ContentBaseJM
        {
            var model = contentDTO!.CreateModel();

            if (contentDTO is IFileContent fileContent)
            {
                _loadFile.CreateFileAsync(fileContent.SectionContent, fileContent.GetUrl(), fileContent.GetPath())
                    .SafeFireAndForget(Print, false);
            }

            var section = new ChapterSection()
            {
                BookChapterId = chapterId,
                ChoiceFlow = flow,
                Number = number,
                ContentType = contentDTO.GetContentType(),
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
                _loadFile.CreateFileAsync(fileContent.SectionContent, fileContent.GetUrl(), fileContent.GetPath())
                    .SafeFireAndForget(Print, false);

                //var dbContent = await UseContentType.GetContent(_section, dbSection.ContentType).Invoke(dbSection.ContentId);
                _loadFile.DeleteFile(fileContent.GetPath(), fileContent.GetUrl());
            }

            dbSection.ContentType = contentDTO.GetContentType();

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
                _loadFile.DeleteFile(fileContent.GetPath(), fileContent.GetUrl());
            }

            _section.DeleteContent(dbSection.ContentBase);

            _section.DeleteSection(dbSection);
        }


        private void Print(Exception ex)
        {
            _logger.LogError($"Ошибка при сохранение данных. Ошибка: {ex}");
        }

        public async Task AddSectionToRedisAsync(string userId, int number, int flow, string serviceKey, object value)
        {
            var curService = _serviceProvider.GetRequiredKeyedService<ICudOperation>(serviceKey);
            await curService.CreateSectionAsync(userId, number, flow, value);
        }
        
        public async Task UpdateSectionToRedisAsync(string userId, int number, int flow, string serviceKey, object value)
        {
            var curService = _serviceProvider.GetRequiredKeyedService<ICudOperation>(serviceKey);
            await curService.UpdateSectionAsync(userId, number, flow, value);
        }
        
        public async Task DeleteSectionFromRedisAsync(string userId, int number, int flow)
        {
            var curService = _serviceProvider.GetRequiredService<BaseCudService>();
            await curService.DeleteSectionFromRedisAsync(userId, number, flow);
        }
    }
}
