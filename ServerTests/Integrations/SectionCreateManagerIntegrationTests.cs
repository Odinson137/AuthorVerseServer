using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.Data.JsonModels;
using StackExchange.Redis;

namespace ServerTests.Integrations
{
    public class SectionCreateManagerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly IDatabase _redis;
        private readonly IConnectionMultiplexer _redisService;

        public SectionCreateManagerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            var configuration = factory.Services.GetRequiredService<IConfiguration>();
            _redisService = factory.Services.GetRequiredService<IConnectionMultiplexer>();
            _redis = _redisService.GetDatabase();

            var token = new CreateJWTtokenService(configuration);
            _client = factory.CreateClient();

            string jwtToken = token.GenerateJwtToken("admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        private async Task ClearTable()
        {
            var endpoints = _redisService.GetEndPoints();
            var server = _redisService.GetServer(endpoints.First());

            var keys = server.Keys();
            foreach (var key in keys)
            {
                await _redis.KeyDeleteAsync(key);
            }
        }

        private async Task AddValueToRedisAsync(int number, int flow)
        {
            var testContent = new TextContentJM()
            {
                SectionContent = "test",
                Operation = ChangeType.Create,
            };

            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            string value = JsonConvert.SerializeObject(testContent, settings);
            await _redis.SortedSetAddAsync($"manager:admin", $"{number}:{flow}", number, flags: CommandFlags.FireAndForget);
            await InitManager();
            await _redis.StringSetAsync($"content:admin:{number}:{flow}", value, flags: CommandFlags.FireAndForget);
        }

        private Task InitManager(int chapterId = 1)
        {
            return _redis.StringSetAsync($"managerInfo:admin", chapterId, flags: CommandFlags.FireAndForget);
        }

        private async Task AddImageToRedisAsync(int number, int flow)
        {
            var testContent = new ImageContentJM()
            {
                SectionContent = await File.ReadAllBytesAsync(@"../../../Images/javascript-it-юмор-geek-5682739.jpeg"),
                Expansion = ".jpeg",
                Operation = ChangeType.Create,
            };

            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            string value = JsonConvert.SerializeObject(testContent, settings);
            await _redis.SortedSetAddAsync($"manager:admin", $"{number}:{flow}", number, flags: CommandFlags.FireAndForget);
            await _redis.StringSetAsync($"managerInfo:admin", 1, flags: CommandFlags.FireAndForget);
            await _redis.StringSetAsync($"content:admin:{number}:{flow}", value, flags: CommandFlags.FireAndForget);
        }

        private async Task AddTextToDbAsync(int number, int flow)
        {
            var context = await _client.PostAsync($"/api/ChapterSection/CreateTextSection?number={number}&flow={flow}&text=test", null);
            await SaveAsync();
        }

        private async Task DeleteTextFromRedisAsync(int number, int flow)
        {
            var deleteContent = new ContentBaseJM()
            {
                Operation = ChangeType.Delete,
            };

            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            string value = JsonConvert.SerializeObject(deleteContent, settings);
            await _redis.SortedSetAddAsync($"manager:admin", $"{number}:{flow}", number, flags: CommandFlags.FireAndForget);
            await InitManager();
            await _redis.StringSetAsync($"content:admin:{number}:{flow}", value, flags: CommandFlags.FireAndForget);
        }

        private async Task SaveAsync()
        {
            var response = await _client.PostAsync($"/api/ChapterSection/FinallySave", null);
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
        public async Task CreateNewImageSection_CheckDataInRedis_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await AddValueToRedisAsync(1, 1);

            const int number = 11;
            const int flow = 1;

            var contentFile = new MultipartFormDataContent();

            await using (var fileStream = File.OpenRead(@"../../../Images/javascript-it-юмор-geek-5682739.jpeg"))
            {
                var fileContent = new StreamContent(fileStream);
                contentFile.Add(fileContent, "imageFile", "javascript-it-юмор-geek-5682739.jpeg");
                // Act
                var response = await _client
                    .PostAsync($"/api/ChapterSection/CreateImageSection?number={number}&flow={flow}", contentFile);
                var content = await response.Content.ReadAsStringAsync();
            
                Assert.True(response.IsSuccessStatusCode);
            }
            var isExistContent = await _redis.KeyExistsAsync($"content:admin:{number}:{flow}");

            // Assert
            Assert.True(isExistContent);
        }

        /// <summary>
        /// если прошлую секцию с такими flow и number пытались удалить и затем на её месте создать новую
        /// то из операций delete и create, она переходит в операцию update
        /// </summary>
        [Fact]
        public async Task CreateNewTextSection_FromDeleteToUpdateRedis_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await InitManager();
            await DeleteTextFromRedisAsync(10, 1);

            // Act
            var response =
                await _client.PostAsync("/api/ChapterSection/CreateTextSection?number=10&flow=1&text=test", null);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            var existContent = await _redis.StringGetAsync($"content:admin:10:1");

            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var redisContent = JsonConvert.DeserializeObject<ContentBaseJM>(existContent!, settings);
            
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(redisContent);
            Assert.True(redisContent.Operation == ChangeType.Update);
        }

        [Fact]
        public async Task CreateNewTextSection_FromDeleteToUpdateToDb_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await InitManager();
            await DeleteTextFromRedisAsync(10, 1);

            // Act
            var response = await _client
                .PostAsync($"/api/ChapterSection/CreateTextSection?number=10&flow=1&text=контент должен остаться, но текст измениться", null);
            var content = await response.Content.ReadAsStringAsync();

            var existContent = await _redis.StringGetAsync($"content:admin:10:1");
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var redisContent = JsonConvert.DeserializeObject<ContentBaseJM>(existContent!, settings);

            await SaveAsync();

            var existContentAfterSaving = await _redis.KeyExistsAsync($"content:admin:10:1");

            // Assert

            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(redisContent);
            Assert.True(redisContent.Operation == ChangeType.Update);
            
            Assert.False(existContentAfterSaving);
        }

        [Fact]
        public async Task SaveSectionFromManager_Ok_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await AddImageToRedisAsync(11, 1);

            // Act
            var response = await _client.PostAsync($"/api/ChapterSection/FinallySave", null);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
        }


        [Fact]
        public async Task DeleteSection_DeleteFromRedis_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await AddValueToRedisAsync(11, 1);

            // Act
            var response = await _client.DeleteAsync($"/api/ChapterSection/DeleteSection?number=11&flow=1");
            var content = await response.Content.ReadAsStringAsync();

            var isExistContent = await _redis.KeyExistsAsync($"content:admin:11:1");
            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.False(isExistContent);
        }


        [Fact]
        public async Task DeleteSection_DeleteFromDb_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await InitManager();
            await AddTextToDbAsync(11, 1);

            // Act
            var response = await _client.DeleteAsync($"/api/ChapterSection/DeleteSection?number=11&flow=1");
            var content = await response.Content.ReadAsStringAsync();
            
            await SaveAsync();

            var isExistContent = await _redis.KeyExistsAsync($"content:admin:11:1");
            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.False(isExistContent);
        }
    }
}
