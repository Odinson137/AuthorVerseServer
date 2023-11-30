using AuthorVerseForum.DTO;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using AuthorVerseForum.Services;
using AuthorVerseForum.Data.Enums;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace AuthorVerseForum.Hubs
{
    //[Authorize]
    public class ForumHub : Hub
    {
        private readonly IDatabase _redis;

        private readonly ILogger _logger;
        private readonly UncodingJwtoken _jwtoken;

        public ForumHub(IConnectionMultiplexer redisConnection, UncodingJwtoken jwtoken)
        {
            _redis = redisConnection.GetDatabase();
            _jwtoken = jwtoken;
        }

        private string GetConnectionId()
        {
            return $"connectionForum:{Context.ConnectionId}";
        }

        private string GetViewUserName(UserVerify user)
        {
            if (string.IsNullOrEmpty(user.Name))
                return user.UserName;
            else
                return $"{user.Name} {user.LastName}";

        }

        public async Task SendMessage(string message)
        {
            string? connectorJson = await _redis.StringGetAsync(GetConnectionId());
            Console.WriteLine($"Cinnector: {connectorJson}");

            if (string.IsNullOrEmpty(connectorJson))
            {
                await Clients.Caller.SendAsync("ErrorMessage", 404, "User data is not exist");
                return;
            }

            var connecter = JsonConvert.DeserializeObject<ConnecterDTO>(connectorJson);
            if (connecter == null)
            {
                await Clients.Caller.SendAsync("ErrorMessage", 500, "Server Error");
                return;
            }

            string? userJson = await _redis.StringGetAsync($"session:{connecter.UserId}");
            if (userJson == null)
            {
                await Clients.Caller.SendAsync("ErrorMessage", 500, "Server Error");
                return;
            }

            var user = JsonConvert.DeserializeObject<UserVerify>(userJson);

            await Clients.Group($"group:{connecter.BookId}").SendAsync("ReceiveMessage", new MessageSendDTO
            { 
                BookId = connecter.BookId,
                Text = message
            }, new UserMessageDTO(user, connecter.UserId));
        }

        public async Task ChangeUserStatus(UserStatus status)
        {
            string? connectorJson = await _redis.StringGetAsync(GetConnectionId());
            if (string.IsNullOrEmpty(connectorJson))
            {
                await Clients.Caller.SendAsync("ErrorMessage", 404, "User data is not exist");
                return;
            }

            var connecter = JsonConvert.DeserializeObject<ConnecterDTO>(connectorJson);
            if (connecter == null)
            {
                await Clients.Caller.SendAsync("ErrorMessage", 500, "Server Error");
                return;
            }

            string? userJson = await _redis.StringGetAsync($"session:{connecter.UserId}");
            if (userJson == null)
            {
                await Clients.Caller.SendAsync("ErrorMessage", 500, "Server Error");
                return;
            }

            var user = JsonConvert.DeserializeObject<UserVerify>(userJson);
            if (user == null)
            {
                await Clients.Caller.SendAsync("ErrorMessage", 500, "Server Error");
                return;
            }

            await Clients.Group($"group:{connecter.BookId}").SendAsync("ChangeUserStatus", GetViewUserName(user), status);
        }

        public async Task AuthorizationConnection(string token, int bookId)
        {
            Console.WriteLine("Личные данные пользователя:");
            Console.WriteLine(token);
            Console.WriteLine(bookId);

            Console.WriteLine("Расшифровка токена:");

            string userId = _jwtoken.GetUserId(token); 
            Console.WriteLine(userId);

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token) || bookId <= 0)
            {
                await DisconnectWithError("Недостаточно данных для подключения");
                return;
            }

            var userJson = await _redis.StringGetAsync($"session:{userId}");
            Console.Write("Сессия пользователя после авторизации: ");
            Console.WriteLine(userJson);

            if (string.IsNullOrEmpty(userJson))
            {
                await DisconnectWithError("Данные пользователя не найдены в сессии.");
                return;
            }

            var connecter = new ConnecterDTO()
            {
                UserId = userId,
                BookId = bookId,
            };

            await Groups.AddToGroupAsync(Context.ConnectionId, $"group:{bookId}");

            var connectionId = GetConnectionId();
            await _redis.StringSetAsync(connectionId, JsonConvert.SerializeObject(connecter));

            var count = await _redis.StringIncrementAsync($"forumCount:{bookId}");
            Console.WriteLine("Count - " + count);
            await Clients.Group($"group:{bookId}").SendAsync("UserCountMessage", count);
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("Подключился новый пользователь!");
            await base.OnConnectedAsync();
        }

        private async Task DisconnectWithError(string errorMessage)
        {
            Console.WriteLine(errorMessage);
            await base.OnDisconnectedAsync(new HubException(errorMessage));
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectorJson = await _redis.StringGetAsync(GetConnectionId());
            if (!string.IsNullOrEmpty(connectorJson))
            {
                var connector = JsonConvert.DeserializeObject<ConnecterDTO>(connectorJson);
                await _redis.KeyDeleteAsync(GetConnectionId());
                var count = await _redis.StringDecrementAsync($"forumCount:{connector.BookId}");
                await Clients.All.SendAsync("UserCountMessage", count);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"group:{connector.BookId}");
            }

            Console.WriteLine($"DicConnected");

            await base.OnDisconnectedAsync(exception);
        }
    }
}
