using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AuthorVerseServer.DTO;
using Newtonsoft.Json;
using System.Net;
using StackExchange.Redis;
using AuthorVerseServer.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Net.Mime.MediaTypeNames;

namespace ServerTests.Integrations
{
    public class SectionCreateManagerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly IDatabase _redis;

        public SectionCreateManagerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            var configuration = factory.Services.GetRequiredService<IConfiguration>();
            var redisService = factory.Services.GetRequiredService<IConnectionMultiplexer>();
            _redis = redisService.GetDatabase();

            var token = new CreateJWTtokenService(configuration);
            _client = factory.CreateClient();

            string jwtToken = token.GenerateJwtToken("admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        [Fact]
        public async Task CreateBookManager_AddNullDataToRedis_ReturnsOkResult()
        {
            // Arrange
            await _redis.KeyDeleteAsync($"manager:admin");

            // Act
            var response = await _client.PostAsync($"/api/ChapterSection/CreateManager", null);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }


        [Fact]
        public async Task CreateBookManager_AddDLastManagerFromRedis_ReturnsOkResult()
        {
            // Arrange
            await _redis.KeyDeleteAsync($"manager:admin");
            await _redis.KeyDeleteAsync($"content:admin:1:1");
            var testContent = new TextContent()
            {
                SectionContent = "test",
                Operation = AuthorVerseServer.Data.Enums.ChangeType.Create,
                Type = AuthorVerseServer.Data.Enums.ContentType.Text,
            };

            string value = JsonConvert.SerializeObject(testContent);
            await _redis.SortedSetAddAsync($"manager:admin", 1, 1, flags: CommandFlags.FireAndForget);
            await _redis.StringSetAsync($"content:admin:1:1", value, TimeSpan.FromSeconds(10), flags: CommandFlags.FireAndForget);

            // Act
            var response = await _client.PostAsync($"/api/ChapterSection/CreateManager", null);
            var content = await response.Content.ReadAsStringAsync();
            var contents = JsonConvert.DeserializeObject<ICollection<string>>(content);

            var isExistManager = await _redis.KeyExistsAsync($"manager:admin");
            var isExistContent = await _redis.KeyExistsAsync($"content:admin:1:1");
            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.True(isExistManager);
            Assert.True(isExistContent);
            Assert.NotNull(contents);
            Assert.True(contents.Count > 0);
        }
    }
}
