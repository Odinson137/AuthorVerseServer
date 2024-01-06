using AsyncAwaitBestPractices;
using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.Data.JsonModels;
using AuthorVerseServer.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace AuthorVerseServer.Services;

public class BaseCudService
{
    private readonly IDatabase _redis;
    private readonly ICreator _creator;

    public BaseCudService(IConnectionMultiplexer connectionMultiplexer, ICreator creator)
    {
        _redis = connectionMultiplexer.GetDatabase();
        _creator = creator;
    }
    
    private protected async Task<ChangeType> CheckCreateNewContentAsync(string userId, int number, int flow)
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

        // если ли есть текущий элемент и если это удаление, то гуд, а если нет, то ошибка
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
        else if (checkBeforeAsync.IsNullOrEmpty)
        {
            var checkDb = await _creator.CheckAddingNewSectionAsync(chapterId, flow);
            if (checkDb != number - 1)
            {
                throw new Exception("The section cannot be added to the db");
            }
        }

        return changeType;
    }
    
    private protected async Task CheckUpdateNewContentAsync(string userId, int number, int flow)
    {
        // достаём номер главы
        var managerInfo = await _redis.StringGetAsync($"managerInfo:{userId}");
        if (!int.TryParse(managerInfo, out int chapterId))
        {
            throw new Exception("The creating session has time out");
        }
            
        // если ли есть текущий элемент и если это удаление, то плохо, так как нельзя редактировать то, что было удалено - ошибка
        var checkContent = await _redis.StringGetAsync($"content:{userId}:{number}:{flow}");

        if (checkContent.HasValue)
        {
            if (JObject.Parse(checkContent!)["operation"]!.ToObject<ChangeType>() == ChangeType.Delete)
            {
                throw new Exception("The section with this number and in this flow already exists");
            }
        }
        else if (await _creator.CheckUpdatingNewSectionAsync(chapterId, number, flow) == false)
        {
            throw new Exception("The section cannot be added to the db");
        }
    }

    internal async ValueTask DeleteSectionFromRedisAsync(string userId, int number, int flow)
    {
        var managerInfo = await _redis.StringGetAsync($"managerInfo:{userId}");
        if (!int.TryParse(managerInfo, out int chapterId))
        {
            throw new Exception("The creating session has time out");
        }

        var existNextKey = await _redis.KeyExistsAsync($"content:{userId}:{number+1}:{flow}");

        if (existNextKey)
        {
            throw new Exception("This is no the last section");
        }

        var content = await _redis.StringGetAsync($"content:{userId}:{number}:{flow}");

        if (content.IsNullOrEmpty)
        {
            // если в редисе ничего не содержится, то проверить в бд есть ли такой элемент и добавить в редис контент на удаление
            if (await _creator.CheckAddingNewSectionAsync(chapterId, flow) != number)
            {
                throw new Exception("The section cannot be deleted from the db");
            }

            // добавить контент на удаление
            var contentBase = new ContentBaseJm()
            {
                Operation = ChangeType.Delete,
            };

            var jsonContent = JsonConvert.SerializeObject(contentBase);

            SetValueToRedisAsync(userId, number, flow, jsonContent);
        } else {
            // если в редисе что-то содержится, значит в бд ничего нет, и можно спокойно удалить данные в редис и всё
            _redis.KeyDeleteAsync($"content:{userId}:{number}:{flow}", flags: CommandFlags.FireAndForget).SafeFireAndForget();
            _redis.SortedSetRemoveAsync($"manager:{userId}", $"{number}:{flow}", flags: CommandFlags.FireAndForget).SafeFireAndForget();
        }
    }
    
    private protected void SetValueToRedisAsync(string userId, int number, int flow, string value)
    {
        _redis.StringSetAsync($"content:{userId}:{number}:{flow}", value,
            flags: CommandFlags.FireAndForget);
        _redis.SortedSetAddAsync($"manager:{userId}", $"{number}:{flow}", number,
            flags: CommandFlags.FireAndForget);
    }
    
    
    private protected byte[] GetBytesFromIFormFile(IFormFile file)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            file.CopyTo(memoryStream);

            byte[] byteArray = memoryStream.ToArray();

            return byteArray;
        }
    }
}