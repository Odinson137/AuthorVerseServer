using AuthorVerseForum.DTO;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using AuthorVerseForum.Services;
using AuthorVerseForum.Data.Enums;
using Newtonsoft.Json;

namespace AuthorVerseForum.Hubs
{
    //[Authorize]
    public class ForumHub : Hub
    {
        private readonly IDatabase _redis;
        private readonly UncodingJwtoken _jwtoken;
        private readonly HttpClient _client;
        private readonly ILogger<ForumHub> _logger;

        public ForumHub(IConnectionMultiplexer redisConnection, UncodingJwtoken jwtoken, HttpClient client, ILogger<ForumHub> logger)
        {
            _redis = redisConnection.GetDatabase();
            _jwtoken = jwtoken;
            _client = client;
            _logger = logger;
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

        private string GetServerUri(string key)
        {
            #if !DEBUG
                string apiUrl = $"http://server:8080/api/ForumMessage?key={key}";
            #else
                string apiUrl = $"http://localhost:7069/api/ForumMessage?key={key}";
            #endif
            return apiUrl;
        }

        public async Task<(int, ConnecterDTO, UserVerify)> GetConnectionInfoAsync()
        {
            string? connectorJson = await _redis.StringGetAsync(GetConnectionId());
            _logger.LogInformation($"Connector: {connectorJson}");

            if (string.IsNullOrEmpty(connectorJson))
            {
                await Clients.Caller.SendAsync("ErrorMessage", 404, "User data is not exist");
                return (0, null, null);
            }

            ConnecterDTO? connecter = JsonConvert.DeserializeObject<ConnecterDTO>(connectorJson);
            if (connecter == null)
            {
                await Clients.Caller.SendAsync("ErrorMessage", 500, "Server Error");
                return (0, null, null);
            }

            string? userJson = await _redis.StringGetAsync($"session:{connecter.UserId}");
            if (userJson == null)
            {
                await Clients.Caller.SendAsync("ErrorMessage", 500, "User session lost");
                return (0, null, null);
            }

            UserVerify? user = JsonConvert.DeserializeObject<UserVerify>(userJson);

            if (user == null)
            {
                await Clients.Caller.SendAsync("ErrorMessage", 500, "Server Error");
                return (0, null, null);
            }

            return (1, connecter, user);
        }

        public async Task SendMessage(string message, AnswerDTO? answerMessage = null)
        {
            var items = await GetConnectionInfoAsync();
            if (items.Item1 == 0) return;

            ConnecterDTO connecter = items.Item2;
            UserVerify user = items.Item3;

            SendForumMessageDTO sendMessage = new SendForumMessageDTO
            {
                BookId = connecter.BookId,
                Text = message,
                AnswerId = answerMessage == null ? null : answerMessage.MessageId,
                UserId = connecter.UserId,
            };

            string key = Guid.NewGuid().ToString();
            await _redis.StringSetAsync($"add_message:{key}", JsonConvert.SerializeObject(sendMessage), TimeSpan.FromSeconds(60));
            
            int messageId = await SendMessageToServerAsync(key);
            if (messageId > 0)
            {
                await Clients.GroupExcept($"group:{connecter.BookId}", Context.ConnectionId).SendAsync("ReceiveMessage", 
                    new MessageSendDTO
                    {
                        MessageId = messageId,
                        BookId = connecter.BookId,
                        Text = message
                    }, new UserMessageDTO(user, connecter.UserId));

                await Clients.Caller.SendAsync("ReturnMessageId", messageId);
            }
            else
            {
                await Clients.Caller.SendAsync("ErrorMessage", 500, "Message not sent");
            }

            await _redis.KeyDeleteAsync($"add_message:{key}");
        }

        private async Task<int> SendMessageToServerAsync(string key)
        {
            var response = await _client.PostAsync(GetServerUri(key), null);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Сообщение успешно отправлено!");
                var content = await response.Content.ReadAsStringAsync();
                return int.Parse(content);
            }
            else
            {
                _logger.LogError($"Ошибка: {response.Content}");
                return 0;
            }
        }

        private async Task<int> SendMessageToPutTextAsync(string key)
        {
            var response = await _client.PutAsync(GetServerUri(key), null);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Сообщение успешно изменено!");
                return 1;
            }
            else
            {
                _logger.LogError($"Ошибка: {response.Content}");
                return 0;
            }
        }

        private async Task<int> SendMessageToDeleteAsync(string key)
        {
            var response = await _client.DeleteAsync(GetServerUri(key));

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Сообщение успешно удалено!");
                return 1;
            }
            else
            {
                _logger.LogError($"Ошибка: {response.Content}");
                return 0;
            }
        }

        public async Task ChangeUserStatus(UserStatus status)
        {
            var items = await GetConnectionInfoAsync();
            if (items.Item1 == 0) return;

            ConnecterDTO connecter = items.Item2;
            UserVerify user = items.Item3;

            await Clients.GroupExcept($"group:{connecter.BookId}", Context.ConnectionId)
                .SendAsync("ChangeStatus", GetViewUserName(user), status);
        }

        public async Task ChangeTextMessage(int messageId, string newMessageText)
        {
            var items = await GetConnectionInfoAsync();
            if (items.Item1 == 0) return;

            ConnecterDTO connecter = items.Item2;

            await Clients.GroupExcept($"group:{connecter.BookId}", Context.ConnectionId)
                .SendAsync("ChangeText", messageId, newMessageText);

            string key = new Guid().ToString();
            await _redis.StringSetAsync($"put_message:{key}", 
                JsonConvert.SerializeObject(new { MessageId = messageId, NewText = newMessageText }), 
                TimeSpan.FromSeconds(60));

            if (await SendMessageToPutTextAsync(key) == 0)
            {
                await Clients.Caller.SendAsync("ErrorMessage", 500, "Error when saving data to the database");
            }
        }

        public async Task DeleteMessage(int messageId)
        {
            var items = await GetConnectionInfoAsync();
            if (items.Item1 == 0) return;

            ConnecterDTO connecter = items.Item2;

            await Clients.GroupExcept($"group:{connecter.BookId}", Context.ConnectionId)
                .SendAsync("DeleteMessage", messageId);

            string key = new Guid().ToString();
            await _redis.StringSetAsync($"delete_message:{key}",
                JsonConvert.SerializeObject(new { MessageId = messageId, UserId = connecter.UserId }),
                TimeSpan.FromSeconds(60));

            if (await SendMessageToDeleteAsync(key) == 0)
            {
                await Clients.Caller.SendAsync("ErrorMessage", 500, "Error when deleting data to the database");
            }
        }

        public async Task AuthorizationConnection(string token, int bookId)
        {
            _logger.LogWarning("Личные данные пользователя:");
            _logger.LogInformation($"Token - {token}");
            _logger.LogInformation($"Book id - {bookId}");
            
            string userId = _jwtoken.GetUserId(token); 

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token) || bookId <= 0)
            {
                _logger.LogError("Недостаточно данных для подключения");
                await DisconnectWithError("Недостаточно данных для подключения");
                return;
            }

            var userJson = await _redis.StringGetAsync($"session:{userId}");

            if (string.IsNullOrEmpty(userJson))
            {
                _logger.LogError("Данные пользователя не найдены в сессии.");
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
            _logger.LogInformation("Count - " + count);
            await Clients.Group($"group:{bookId}").SendAsync("UserCountMessage", count);
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Подключился новый пользователь!");
            await base.OnConnectedAsync();
        }

        private async Task DisconnectWithError(string errorMessage)
        {
            _logger.LogError(errorMessage);
            await base.OnDisconnectedAsync(new HubException(errorMessage));
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectorJson = await _redis.StringGetAsync(GetConnectionId());
            if (!string.IsNullOrEmpty(connectorJson))
            {
                var connector = JsonConvert.DeserializeObject<ConnecterDTO>(connectorJson!);
                await _redis.KeyDeleteAsync(GetConnectionId());
                var count = await _redis.StringDecrementAsync($"forumCount:{connector!.BookId}");
                await Clients.All.SendAsync("UserCountMessage", count);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"group:{connector.BookId}");
            }

            _logger.LogWarning("DicConnected");

            await base.OnDisconnectedAsync(exception);
        }
    }
}
