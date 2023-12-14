using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using AuthorVerseServer.DTO;
using Newtonsoft.Json;
using System.Text;
using StackExchange.Redis;
using Microsoft.AspNetCore.SignalR.Client;

namespace ForumHubTests.Integrations
{
    public class ForumIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly IDatabase _redis;
        private readonly WebApplicationFactory<Program> _factory;
        public ForumIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            var redisConnection = factory.Services.GetRequiredService<IConnectionMultiplexer>();

            _redis = redisConnection.GetDatabase();
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ForumHub_LoginUser_Ok()
        {
            // Arrange
            UserLoginDTO user = new UserLoginDTO()
            {
                UserName = "admin",
                Password = "Password@123",
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/User/Login", requestContent);
            var token = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.False(string.IsNullOrEmpty(token));
            string? session = await _redis.StringGetAsync("session:admin");
            Assert.False(string.IsNullOrEmpty(session));
            var userVerify = JsonConvert.DeserializeObject<UserVerify>(session);
            Assert.True(userVerify != null);
        }

        public async Task<string> GetTokenAsync()
        {
            UserLoginDTO user = new UserLoginDTO()
            {
                UserName = "admin",
                Password = "Password@123",
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/User/Login", requestContent);

            return await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async Task ForumHub_SussecfullyConnected_Ok()
        {
            // Arrange

            // Act
            var connection = new HubConnectionBuilder()
                .WithUrl($"http://localhost:7070/forum")
                .Build();

            // Assert
            await connection.StartAsync();

            Assert.Equal(HubConnectionState.Connected, connection.State);
        }

        [Fact]
        public async Task ForumHub_SussecfullyAuthenfication_Ok()
        {
            // Arrange
            string token = await GetTokenAsync();
            int bookId = 1;

            // Act
            var connection = new HubConnectionBuilder()
                .WithUrl($"http://localhost:7070/forum")
                .Build();

            // Assert
            await connection.StartAsync();

            var countBefore = await _redis.StringGetAsync($"forumCount:{bookId}");
            await connection.InvokeAsync("AuthorizationConnection", token, bookId);
            var countLater = await _redis.StringGetAsync($"forumCount:{bookId}");

            // Assert
            Assert.True(connection.State == HubConnectionState.Connected);
            Assert.False(string.IsNullOrEmpty(countBefore));
            Assert.False(string.IsNullOrEmpty(countLater));
            Assert.True(countBefore != countLater);
        }

        public async Task<HubConnection> AuthorizeAsync()
        {
            string token = await GetTokenAsync();
            int bookId = 1;

            var connection = new HubConnectionBuilder()
                .WithUrl($"http://localhost:7070/forum")
                .Build();
            await connection.StartAsync();
            await connection.InvokeAsync("AuthorizationConnection", token, bookId);
            return connection;
        }

        [Fact]
        public async Task SendMessage_SendMessageWithNullParrent_Ok()
        {
            // Arrange
            var connection = await AuthorizeAsync();
            string message = "Hello from test";

            // Assert
            await connection.InvokeAsync("SendMessage", message, null);

            // Assert
            Assert.True(connection.State == HubConnectionState.Connected);
        }

        [Fact]
        public async Task SendMessage_SendMessageWithParrent_Ok()
        {
            // Arrange
            var connection = await AuthorizeAsync();
            string message = "Hello from test";

            var answerMessage = new { MessageId = 1, ViewName = "Yuri Test", Text = "Это мой первый тести..." };

            // Assert
            await connection.InvokeAsync("SendMessage", message, answerMessage);

            // Assert
            Assert.True(connection.State == HubConnectionState.Connected);
        }

        [Fact]
        public async Task ChangeMessageText_ReturnOne_Ok()
        {
            // Arrange
            var connection = await AuthorizeAsync();
            string message = "Hello from forum: change text";

            // Assert
            await connection.InvokeAsync("ChangeTextMessage", 1, message);

            // Assert
            Assert.True(connection.State == HubConnectionState.Connected);
        }

        private async Task<int> GetNewMessageIdAsync()
        {
            string newGuid = Guid.NewGuid().ToString();
            string key = $"add_message:{newGuid}";

            SendForumMessageDTO sendMessage = new SendForumMessageDTO
            {
                BookId = 1,
                Text = "Hello",
                UserId = "admin",
                AnswerId = null,
            };

            var redisConnection = _factory.Services.GetRequiredService<IConnectionMultiplexer>();
            var redis = redisConnection.GetDatabase();
            await redis.StringSetAsync(key, JsonConvert.SerializeObject(sendMessage), TimeSpan.FromSeconds(10));

            var response = await _client.PostAsync($"/api/ForumMessage?key={newGuid}", null);
            var content = await response.Content.ReadAsStringAsync();
            int.TryParse(content, out int value);
            return value;
        }

        [Fact]
        public async Task DeleteUserMessage_ReturnOk_Ok()
        {
            // Arrange
            var connection = await AuthorizeAsync();

            int messageId = await GetNewMessageIdAsync();

            // Assert
            await connection.InvokeAsync("DeleteMessage", messageId);

            // Assert
            Assert.True(connection.State == HubConnectionState.Connected);
        }
    }
}