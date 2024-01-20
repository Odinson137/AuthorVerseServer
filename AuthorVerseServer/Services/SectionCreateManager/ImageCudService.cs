using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.Data.JsonModels;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Interfaces.ServiceInterfaces.SectionCreateManager;
using Newtonsoft.Json;
using StackExchange.Redis;
using Path = System.IO.Path;

namespace AuthorVerseServer.Services;

public class ImageCudService : BaseCudService, ICudOperations
{
    public ImageCudService(IConnectionMultiplexer redis, ICreator creator) : base(redis, creator)
    {
    }
    
    public async ValueTask CreateSectionAsync(string userId, int number, int flow, object value)
    {
        var changeType = await CheckCreateNewContentAsync(userId, number, flow);

        var file = (IFormFile)value;
        var content = new ImageContentJm()
        {
            SectionContent = GetBytesFromIFormFile(file),
            Expansion = Path.GetExtension(file.FileName),
            Operation = changeType,
        };

        string jsonContent = JsonConvert.SerializeObject(content,
            new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

        SetValueToRedisAsync(userId, number, flow, jsonContent);
    }

    public async ValueTask UpdateSectionAsync(string userId, int number, int flow, object value)
    {
        var operation = await CheckUpdateNewContentAsync(userId, number, flow);
        
        var file = (IFormFile)value;
        var content = new ImageContentJm()
        {
            Operation = operation,
            SectionContent = GetBytesFromIFormFile(file),
            Expansion = Path.GetExtension(file.FileName),
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