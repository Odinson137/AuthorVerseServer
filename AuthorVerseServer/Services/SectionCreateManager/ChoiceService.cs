using System.Text.RegularExpressions;
using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.Data.JsonModels;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Interfaces.ServiceInterfaces.SectionCreateManager;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace AuthorVerseServer.Services;

public class ChoiceService : ICudChoiceOperations
{
    private readonly IDatabase _redis;
    private readonly ICreator _creator;
    
    public ChoiceService(IConnectionMultiplexer redisConnection, ICreator creator)
    {
        _creator = creator;
        _redis = redisConnection.GetDatabase();
    }
    
    public async ValueTask CreateChoiceAsync(string userId, int number, int flow, int choiceNumber, 
        int nextChapterId, int nextNumber, int nextFlow, string text)
    {
        // достаём номер главы
        var managerInfo = await _redis.StringGetAsync($"managerInfo:{userId}");
        if (!int.TryParse(managerInfo, out int chapterId))
        {
            throw new Exception("The creating session has time out");
        }

        var redisValue =
            await _redis.HashGetAsync($"choiceManager:{userId}", 
                $"{choiceNumber}:{number}:{flow}:{choiceNumber}");

        var operation = ChangeType.Create;
        if (redisValue.HasValue)
        {
            var currentChoice = JsonConvert.DeserializeObject<ChoiceContent>(redisValue!)!;
            if (currentChoice.Operation == ChangeType.Delete)
            {
                operation = ChangeType.Update;
            }
        }
        else
        {
            var sectionContent = await _redis.StringGetAsync($"content:{userId}:{number}:{flow}");
            if (!sectionContent.HasValue)
            {
                var choicesFlow = await _creator.CheckAddingNewChoiceAsync(chapterId, number, flow);
                if (choicesFlow.Choices.Contains(choiceNumber))
                {
                    throw new Exception("A choice with this number already exists");
                }
            } 
        }

        var content = new ChoiceContent(chapterId)
        {
            Content = text,
            NextChapterId = nextChapterId,
            NextNumber = nextNumber,
            NextFlow = nextFlow,
            Operation = operation,
        };

        await _redis.HashSetAsync($"choiceManager:{userId}", $"{chapterId}:{number}:{flow}:{choiceNumber}", 
            JsonConvert.SerializeObject(content), flags: CommandFlags.FireAndForget);
    }

    public async ValueTask UpdateChoiceAsync(string userId, int number, int flow, int choiceNumber, 
        int nextChapterId, int nextNumber, int nextFlow, string text)
    {
        // достаём номер главы
        var managerInfo = await _redis.StringGetAsync($"managerInfo:{userId}");
        if (!int.TryParse(managerInfo, out int chapterId))
        {
            throw new Exception("The creating session has time out");
        }
        
        var redisValue = 
            await _redis.HashGetAsync($"choiceManager:{userId}", $"{choiceNumber}:{number}:{flow}:{choiceNumber}");

        var operation = ChangeType.Update;
        if (redisValue.HasValue)
        {
            var isDeleted = new Regex($"\"operation\":\\s*{(int)ChangeType.Delete}").Match(redisValue!);
            
            if (isDeleted.Success)
            {
                throw new Exception("The choice was deleted, and you can't update the choice");
            }

            var isCreated = new Regex($"\"operation\":\\s*{(int)ChangeType.Create}").Match(redisValue!);
            if (isCreated.Success)
            {
                operation = ChangeType.Create;
            }
        }
        else
        {
            var choicesFlow = await _creator.CheckAddingNewChoiceAsync(chapterId, number, flow);
            if (!choicesFlow.Choices.Contains(choiceNumber))
            {
                throw new Exception("A choice with this number does not exist");
            }
        }

        var content = new ChoiceContent(chapterId)
        {
            Content = text,
            NextChapterId = nextChapterId,
            NextNumber = nextNumber,
            NextFlow = nextFlow,
            Operation = operation,
        };

        await _redis.HashSetAsync($"choiceManager:{userId}", $"{choiceNumber}:{number}:{flow}:{choiceNumber}", 
            JsonConvert.SerializeObject(content), flags: CommandFlags.FireAndForget);
    }

    public async ValueTask DeleteChoiceAsync(string userId, int number, int flow, int choiceNumber)
    {
        // достаём номер главы
        var managerInfo = await _redis.StringGetAsync($"managerInfo:{userId}");
        if (!int.TryParse(managerInfo, out int chapterId))
        {
            throw new Exception("The creating session has time out");
        }
        
        var redisValue = 
            await _redis.HashGetAsync($"choiceManager:{userId}", $"{choiceNumber}:{number}:{flow}:{choiceNumber}");

        if (redisValue.HasValue)
        {
            var isDeleted = new Regex($"\"operation\":\\s*{(int)ChangeType.Delete}").Match(redisValue!);
            
            if (isDeleted.Success)
            {
                throw new Exception("The choice was deleted, and you can't update the choice");
            }
            
            await _redis.HashDeleteAsync($"choiceManager:{userId}", $"{choiceNumber}:{number}:{flow}");
            return;
        }

        var choicesFlow = await _creator.CheckAddingNewChoiceAsync(chapterId, number, flow);
        if (!choicesFlow.Choices.Contains(choiceNumber))
        {
            throw new Exception("A choice with this number does not exist");
        }
        
        var content = new ChoiceContent(chapterId)
        {
            Operation = ChangeType.Delete,
        };

        await _redis.HashSetAsync($"choiceManager:{userId}", $"{choiceNumber}:{number}:{flow}:{choiceNumber}", 
            JsonConvert.SerializeObject(content), flags: CommandFlags.FireAndForget);
    }
}