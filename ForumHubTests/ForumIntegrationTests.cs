using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using AuthorVerseServer.DTO;
using Newtonsoft.Json;
using System.Text;
using StackExchange.Redis;
using Microsoft.AspNetCore.SignalR.Client;
using System.IdentityModel.Tokens.Jwt;

namespace ForumHubTests
{
    public class ForumIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly IDatabase _redis;
        public ForumIntegrationTests(WebApplicationFactory<Program> factory)
        {
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
            Assert.False(String.IsNullOrEmpty(session));
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
            string token = await GetTokenAsync();
            int bookId = 1;

            // Act
            var connection = new HubConnectionBuilder()
                .WithUrl($"http://localhost:7070/forum")
                .Build();

            // Assert
            await connection.StartAsync();

            Assert.Equal(HubConnectionState.Connected, connection.State);
            //await connection.InvokeAsync("Send", message);
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
    }
}