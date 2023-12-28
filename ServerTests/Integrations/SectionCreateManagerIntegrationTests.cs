using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AuthorVerseServer.DTO;
using Newtonsoft.Json;
using System.Net;
using StackExchange.Redis;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;

namespace ServerTests.Integrations
{
    public class SectionCreateManagerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly IDatabase _redis;
        private readonly IConnectionMultiplexer redisService;

        public SectionCreateManagerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            var configuration = factory.Services.GetRequiredService<IConfiguration>();
            redisService = factory.Services.GetRequiredService<IConnectionMultiplexer>();
            _redis = redisService.GetDatabase();

            var token = new CreateJWTtokenService(configuration);
            _client = factory.CreateClient();

            string jwtToken = token.GenerateJwtToken("admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        private async Task ClearTable()
        {
            var endpoints = redisService.GetEndPoints();
            var server = redisService.GetServer(endpoints.First());

            var keys = server.Keys();
            foreach (var key in keys)
            {
                await _redis.KeyDeleteAsync(key);
            }
        }

        [Fact]
        public async Task CreateBookManager_AddNullDataToRedis_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();

            // Act
            var response = await _client.PostAsync($"/api/ChapterSection/CreateManager/1", null);

            var chapterId = await _redis.KeyExistsAsync("managerInfo:admin");
            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.True(chapterId);
        }

        private async Task AddValueToRedisAsync(int number, int flow)
        {
            var testContent = new TextContent()
            {
                SectionContent = "test",
                Operation = AuthorVerseServer.Data.Enums.ChangeType.Create,
                Type = AuthorVerseServer.Data.Enums.ContentType.Text,
            };

            string value = JsonConvert.SerializeObject(testContent);
            await _redis.SortedSetAddAsync($"manager:admin", number, 1,  flags: CommandFlags.FireAndForget);
            await _redis.StringSetAsync($"managerInfo:admin", 1, flags: CommandFlags.FireAndForget);
            await _redis.StringSetAsync($"content:admin:{number}:{flow}", value, flags: CommandFlags.FireAndForget);
        }

        [Fact]
        public async Task CreateBookManager_AddLastManagerFromRedis_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await AddValueToRedisAsync(1, 1);

            // Act
            var response = await _client.PostAsync($"/api/ChapterSection/CreateManager/1", null);
            var content = await response.Content.ReadAsStringAsync();
            var contents = JsonConvert.DeserializeObject<ICollection<string>>(content);

            var isExistManagerInfo = await _redis.KeyExistsAsync($"managerInfo:admin");
            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.True(isExistManagerInfo);
            Assert.NotNull(contents);
            Assert.True(contents.Count > 0);
        }


        [Fact]
        public async Task CreateNewTextSection_CheckDataInRedis_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await AddValueToRedisAsync(11, 1);

            const int number = 12;
            const int flow = 1;

            // Act
            var response = await _client.PostAsync($"/api/ChapterSection/CreateTextSection?number={number}&flow={flow}&text=test", null);
            var content = await response.Content.ReadAsStringAsync();

            var isExistContent = await _redis.KeyExistsAsync($"content:admin:{number}:{flow}");
            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.True(isExistContent);
        }

        [Fact]
        public async Task CreateNewTextSection_CheckDataInDb_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await AddValueToRedisAsync(1, 1);

            const int number = 11;
            const int flow = 1;

            // Act
            var response = await _client.PostAsync($"/api/ChapterSection/CreateTextSection?number={number}&flow={flow}&text=test", null);
            var content = await response.Content.ReadAsStringAsync();

            var isExistContent = await _redis.KeyExistsAsync($"content:admin:{number}:{flow}");
            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.True(isExistContent);
        }

        [Fact]
        public async Task SaveSectionFromManager_Ok_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await AddValueToRedisAsync(11, 1);

            // Act
            var response = await _client.PostAsync($"/api/ChapterSection/FinallySave", null);
            var content = await response.Content.ReadAsStringAsync();

            var isExistManager = await _redis.KeyExistsAsync($"manager:admin");
            var isExistContent = await _redis.KeyExistsAsync($"content:admin:1:1");
            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.True(isExistManager);
            Assert.True(isExistContent);
        }

        [Fact]
        public async Task DeleteSection_DeleteFromRedis_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await AddValueToRedisAsync(1, 1);

            // Act
            var response = await _client.PostAsync($"/api/ChapterSection/DeleteSection?number=1&flow=1&text=test", null);
            var content = await response.Content.ReadAsStringAsync();

            var isExistManager = await _redis.KeyExistsAsync($"manager:admin");
            var isExistContent = await _redis.KeyExistsAsync($"content:admin:1:1");
            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.True(isExistManager);
            Assert.True(isExistContent);
        }
    }
}
