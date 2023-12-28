using System.Collections;
using AuthorVerseServer.Data.Patterns;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Interfaces.ServiceInterfaces;
using AuthorVerseServer.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace AuthorVerseServer.Services
{
    public class SectionCreateManager : ISectionCreateManager
    {
        private readonly IChapterSection _section;
        private readonly IDatabase _redis;
        public SectionCreateManager(IChapterSection section, IConnectionMultiplexer connectionMultiplexer)
        {
            _section = section;
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

        public async Task ManagerSaveAsync(string userId)
        {
            var manager = await _redis.SortedSetRangeByRankWithScoresAsync($"manager:{userId}");
            //ICollection<ContentBase> collection = new List<ContentBase>();
            foreach (var content in manager)
            {
                var contentValue = await _redis.StringGetAsync($"content:{userId}:{content.Element}:{content.Score}");
                var contentTypeBase = UseContentFromJson.GetContent(contentValue);
                // записывать в файл потом после получение в виде out Название папки!!!
                //collection.Add(contentBase!);
            }
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

        public ValueTask<string> CreateImageSectionAsync(string userId, int number, int flow, IFormFile file)
        {
            throw new NotImplementedException();
        }

    }
}
