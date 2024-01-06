using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.Data.JsonModels;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Interfaces.ServiceInterfaces.SectionCreateManager;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace AuthorVerseServer.Services;

public class TextCudService : BaseCudService, ICudOperations
{
    // private readonly IDatabase _redis;
    
    public TextCudService(IConnectionMultiplexer redis, ICreator creator) : base(redis, creator)
    {
        // _redis = redis;
    }
    
    public async ValueTask CreateSectionAsync(string userId, int number, int flow, object value)
    {
        var changeType = await CheckCreateNewContentAsync(userId, number, flow);

        var content = new TextContentJm()
        {
            SectionContent = (string)value,
            Operation = changeType,
        };

        string jsonContent = JsonConvert.SerializeObject(content,
            new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

        SetValueToRedisAsync(userId, number, flow, jsonContent);
    }

    public async ValueTask UpdateSectionAsync(string userId, int number, int flow, object value)
    {
        await CheckUpdateNewContentAsync(userId, number, flow);

        var content = new TextContentJm()
        {
            Operation = ChangeType.Update,
            SectionContent = (string)value,
        };

        string jsonContent = JsonConvert.SerializeObject(content,
            new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

        SetValueToRedisAsync(userId, number, flow, jsonContent);
    }

    public ValueTask DeleteSectionAsync(string userId, int number, int flow)
    {
        return DeleteSectionFromRedisAsync(userId, number, flow);
    }
    
    

}