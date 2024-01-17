using AsyncAwaitBestPractices;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Grpc.Core;
using GrpcServiceApp;
using Newtonsoft.Json;
using StackExchange.Redis;
using Void = GrpcServiceApp.Void;

namespace AuthorVerseServer.Services.ForumServices;

public class ForumService : Forum.ForumBase
{
    private readonly ILogger<ForumService> _logger;
    private readonly IForumMessage _forum;
    private readonly IDatabase _redis;

    public ForumService(ILogger<ForumService> logger, IForumMessage forum, IConnectionMultiplexer connection)
    {
        _logger = logger;
        _forum = forum;
        _redis = connection.GetDatabase();
    }

    public override async Task<Response> InsertMessage(Request request, ServerCallContext context)
    {
        _logger.LogInformation("start insert method");
        _logger.LogInformation(request.Key);
        
        string? messageJson = await _redis.StringGetAsync($"add_message:{request.Key}");
        
        var sendMessage =  JsonConvert.DeserializeObject<SendForumMessageDTO>(messageJson!)!;
        
        var messageId = await _forum.AddForumMessageProcedureAsync(sendMessage);
        return new Response()
        {
            MessageId = messageId,
        };
    }

    public override async Task<Void?> PatchMessage(Request request, ServerCallContext context)
    {
        _logger.LogInformation("start patch method");
        _logger.LogInformation(request.Key);
     
        var messageJson = await _redis.StringGetAsync($"put_message:{request.Key}");
        
        var changeTextDTO = JsonConvert.DeserializeObject<ChangeTextDTO>(messageJson!)!;
        
        var message = await _forum.GetForumMessageAsync(changeTextDTO.MessageId);
        if (message == null)
        {
            return null;
        }
        message.Text = changeTextDTO.NewText;
        
        await _forum.SaveAsync();

        // _forum.ChangeMessageTextAsync(changeTextDTO.MessageId, changeTextDTO.NewText).SafeFireAndForget();
        
        return new Void();
    }

    public override async Task<Void?> DeleteMessage(Request request, ServerCallContext context)
    {
        _logger.LogInformation("start delete method");
        _logger.LogInformation(request.Key);
        
        string? messageJson = await _redis.StringGetAsync($"delete_message:{request.Key}");
        _logger.LogInformation(messageJson);
        
        var forumMessage = JsonConvert.DeserializeObject<DeleteMessageDTO>(messageJson!)!;

        var isExist = await _forum.CheckUserMessageExistAsync(forumMessage.MessageId, forumMessage.UserId);
        _logger.LogInformation($"Сообщение существует: {isExist}");
        if (!isExist)
        {
            return null;
        }
        
        await _forum.ChangeParentMessage(forumMessage.MessageId);
        await _forum.DeleteMessageAsync(forumMessage.MessageId);
        
        return new Void();
    }
}