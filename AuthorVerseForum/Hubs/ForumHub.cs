using AuthorVerseForum.DTO;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;
using AuthorVerseForum.Services;
using Newtonsoft.Json.Linq;

namespace AuthorVerseForum.Hubs
{
    public class ForumHub : Hub
    {
        private readonly ConnectionManager _connectionManager;
        private readonly IDatabase _redis;

        private readonly ILogger _logger;
        private readonly UncodingJwtoken _jwtoken;

        public ForumHub(ConnectionManager connectionManager, IConnectionMultiplexer redisConnection, UncodingJwtoken jwtoken)
        {
            _connectionManager = connectionManager;
            _redis = redisConnection.GetDatabase();
            _jwtoken = jwtoken;
        }

        private string GetForumUser()
        {
            return $"forumUser-{Context.ConnectionId}";
        }

        private string GetForum(int bookId)
        {
            return $"forum-{bookId}";
        }

        private string GetForumCount(int bookId)
        {
            return $"forumCount-{bookId}";
        }

        public async Task AddToGroup(int bookId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, bookId.ToString());
        }

        public async Task SendMessage(int bookId, string message)
        {
            await Clients.Group($"forum-{bookId}").SendAsync("ReceiveMessage", bookId, message);
        }

        public async Task SendConnectedMessage(int bookId, UserVerify user)
        {
            await _redis.StringSetAsync(GetForumUser(), JsonConvert.SerializeObject(user));

            var count = await _redis.StringIncrementAsync(GetForumCount(bookId));

            await Clients.Group(GetForum(bookId)).SendAsync("UserCountMessage", count);
        }

        public async Task ChangeUserStatus(int bookId, UserVerify user)
        {
            await _redis.StringSetAsync(GetForumUser(), JsonConvert.SerializeObject(user));

            var count = await _redis.StringIncrementAsync(GetForumCount(bookId));

            await Clients.Group(GetForum(bookId)).SendAsync("ChangeUserStatus", count);
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("Подключился новый пользователь!");
            var token = Context.GetHttpContext()?.Request.Query["token"].FirstOrDefault() ?? "";
            if (string.IsNullOrWhiteSpace(token))
            {
                await base.OnDisconnectedAsync((Exception?)null);
                return;
            }

            var userId = _jwtoken.GetUserId(token);
            if (userId == null)
            {
                await base.OnDisconnectedAsync((Exception?)null);
                return;
            }

            var userJson = await _redis.StringGetAsync(userId);
            if (string.IsNullOrEmpty(userJson))
            {
                await base.OnDisconnectedAsync((Exception?)null);
                return;
            }

            //var user = JsonConvert.DeserializeObject<UserVerify>(userJson);

            //await Groups.AddToGroupAsync(Context.ConnectionId, user.);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string userConnectionId = Context.ConnectionId;
            var user = await _redis.StringGetAsync($"");
            Console.WriteLine("DicConnected - " + userConnectionId);
            //await _connectionManager.DeleteUserByConnectionIdAsync(userConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
