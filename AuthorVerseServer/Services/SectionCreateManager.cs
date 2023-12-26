using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Interfaces.ServiceInterfaces;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace AuthorVerseServer.Services
{
    public class SectionCreateManager : ISectionCreateManager
    {
        private readonly IChapterSection _section;
        private readonly IDatabase _redis;
        public SectionCreateManager(IChapterSection section, IDatabase redis)
        {
            _section = section;
            _redis = redis;
        }

        public async ValueTask<SortedSetEntry[]?> CreateManagerAsync(string userId, int chapterId)
        {
            // здесь потому будет множество
            var stringChapterId = await _redis.StringGetAsync($"manager:{userId}");
            if (string.IsNullOrEmpty(stringChapterId))
            {
                await _redis.StringSetAsync(
                    $"manager:{userId}",
                    chapterId,
                    TimeSpan.FromHours(3),
                    flags: CommandFlags.FireAndForget).ConfigureAwait(false);

                return null;
            }
            else
            {
                // отправить пользователю всю информацию об его прошлых изменениях до выхода и повторного входа
                var manager = await _redis.SortedSetRangeByRankWithScoresAsync($"manager:{userId}");
                return manager;
            }
        }

        public async ValueTask<string> CreateTextSectionAsync(string userId, int number, int flow, string text)
        {
            var stringManager = await _redis.StringGetAsync($"manager:{userId}");
            if (stringManager.IsNullOrEmpty)
            {
                return "The creating session has time out";
            }

            var checkContent = await _redis.StringGetAsync($"content:{userId}:{number}:{flow}");

            if (checkContent.HasValue)
            {
                return "The section with this number and in this flow already exists";
            }

            // проверить на возможность добавления данных в бд,
            // то есть узнать не содержится ли элемент с такими параметрами уже в бд 
            // и узнать является ли передаваемый number последующим в коллекции в данном потоке
            if (await _section.CheckAddingNewSectionAsync(number, flow))
            {
                return "The section cannot be added to the db";
            }

            var content = new TextContent()
            {
                SectionContent = text,
                Operation = Data.Enums.ChangeType.Create,
                Type = Data.Enums.ContentType.Text,
            };

            string value = JsonConvert.SerializeObject(content);

            _redis.StringSetAsync($"content:{userId}:{number}:{flow}", value, TimeSpan.FromHours(3), flags: CommandFlags.FireAndForget);
            _redis.SortedSetAddAsync($"manager:{userId}", flow, number, flags: CommandFlags.FireAndForget);

            return string.Empty;
        }
    }
}
