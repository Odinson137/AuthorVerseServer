using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.Data.JsonModels;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Interfaces.ServiceInterfaces.SectionCreateManager;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace AuthorVerseServer.Services;

public class ImageCudService : BaseCudService, ICudOperation
{
    public ImageCudService(IConnectionMultiplexer redis, IChapterSection section) : base(redis, section)
    {
    }
    
    public async ValueTask CreateSectionAsync(string userId, int number, int flow, object value)
    {
        var changeType = await CheckCreateNewContentAsync(userId, number, flow);

        var file = (IFormFile)value;
        var content = new ImageContentJM()
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
        await CheckUpdateNewContentAsync(userId, number, flow);
        
        var file = (IFormFile)value;
        var content = new ImageContentJM()
        {
            Operation = ChangeType.Update,
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