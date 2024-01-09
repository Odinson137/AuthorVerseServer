using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.Data.JsonModels;
using AuthorVerseServer.DTO;
using StackExchange.Redis;

namespace ServerTests.Integrations
{
    public class SectionCreatorIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly IDatabase _redis;
        private readonly IConnectionMultiplexer _redisService;

        public SectionCreatorIntegrationTests(WebApplicationFactory<Program> factory)
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

        private async Task<HttpResponseMessage> AddTextSectionAsync(int number, int flow)
        {
            var response =
                await _client.PostAsync($"/api/Creator/CreateTextSection?number={number}&flow={flow}&text=test", null);

            return response;
        }

        private async Task<HttpResponseMessage> UpdateTextSectionAsync(int number, int flow)
        {
            var response =
                await _client.PutAsync($"/api/Creator/UpdateTextSection?number={number}&flow={flow}&text=test", null);

            return response;
        }
        
        private Task<bool> InitManager(int chapterId = 1)
        {
            return _redis.StringSetAsync($"managerInfo:admin", chapterId, flags: CommandFlags.FireAndForget);
        }

        private async Task<HttpResponseMessage> AddImageSectionAsync(int number, int flow)
        {
            var contentFile = new MultipartFormDataContent();

            await using (var fileStream = File.OpenRead(@"../../../Images/javascript-it-юмор-geek-5682739.jpeg"))
            {
                var fileContent = new StreamContent(fileStream);
                contentFile.Add(fileContent, "imageFile", "javascript-it-юмор-geek-5682739.jpeg");
                // Act
                var response = await _client
                    .PostAsync($"/api/Creator/CreateImageSection?number={number}&flow={flow}", contentFile);

                return response;
            }
        }
        
        private async Task<HttpResponseMessage> DeleteValueAsync(int number, int flow)
        {
            return await _client.DeleteAsync($"/api/Creator/DeleteSection?number={number}&flow={flow}");
        }

        private Task<HttpResponseMessage> SaveAsync()
        {
            return _client.PostAsync($"/api/Creator/FinallySave", null);
        }

        [Fact]
        public async Task ChangeVisibility_ChangeVisibility_ReturnsOkResult()
        {
            // Arrange
            await InitManager();

            var number = 1;
            var flow = 1;
            var newValue = false;
            
            // Act
            var response = 
                await _client.PatchAsync($"/api/Creator/ChangeVisibility?" +
                                        $"number={number}&flow={flow}&newValue={newValue}", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        
        [Fact]
        public async Task CreateBookManager_AddNullDataToRedis_ReturnsNoContent()
        {
            // Arrange
            await ClearTable();

            // Act
            var response = await _client.PostAsync($"/api/Creator/CreateManager/1", null);

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
            await InitManager();
            await DeleteValueAsync(11, 1);
            await SaveAsync(); 
            await AddTextSectionAsync(11, 1);

            // Act
            var response = await _client.PostAsync($"/api/Creator/CreateManager/1", null);
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
            await InitManager();
            await DeleteValueAsync(11, 1);
            await SaveAsync();

            const int number = 11;
            const int flow = 1;

            // Act
            var response = await AddTextSectionAsync(number, flow);
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
            await InitManager();
            
            const int number = 11;
            const int flow = 1;

            // Act
            var response = 
                await _client.PostAsync($"/api/Creator/CreateTextSection?number={number}&flow={flow}&text=test", null);
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
            await InitManager();
            
            const int number = 11;
            const int flow = 1;

            // Act
            var response = await AddImageSectionAsync(number, flow);
            Assert.True(response.IsSuccessStatusCode);

            var isExistContent = await _redis.KeyExistsAsync($"content:admin:{number}:{flow}");

            // Assert
            Assert.True(isExistContent);
        }

        [Fact]
        public async Task CreateNewImageSection_AddChoiceInDb_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await InitManager();
            
            const int number = 11;
            const int flow = 1;

            // Act
            var response = await AddImageSectionAsync(number, flow);

            var saveResponse = await SaveAsync();

            // Assert
            Assert.True(saveResponse.IsSuccessStatusCode);
            await DeleteValueAsync(number, flow);
            await SaveAsync();
            
            Assert.True(response.IsSuccessStatusCode);

            var isExistContent = await _redis.KeyExistsAsync($"content:admin:{number}:{flow}");
            Assert.False(isExistContent);

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
            await DeleteValueAsync(10, 1);

            // Act
            var response = await AddTextSectionAsync(10, 1);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            var existContent = await _redis.StringGetAsync($"content:admin:10:1");

            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var redisContent = JsonConvert.DeserializeObject<ContentBaseJm>(existContent!, settings);
            
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
            await DeleteValueAsync(10, 1);

            // Act
            var response = await AddTextSectionAsync(10, 1);
            var content = await response.Content.ReadAsStringAsync();

            var existContent = await _redis.StringGetAsync($"content:admin:10:1");
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var redisContent = JsonConvert.DeserializeObject<ContentBaseJm>(existContent!, settings);

            await SaveAsync();

            // Assert

            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(redisContent);
            Assert.True(redisContent.Operation == ChangeType.Update);
        }

        [Fact]
        public async Task SaveSectionFromManager_Ok_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await InitManager();
            await AddImageSectionAsync(11, 1);

            // Act
            var response = await SaveAsync();
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.True(response.IsSuccessStatusCode);

            await DeleteValueAsync(11, 1);
            await SaveAsync();
        }

        
        [Fact]
        public async Task SaveSectionFromManager_NothingToSaveError_ReturnsBadRequest()
        {
            // Arrange
            await ClearTable();
            await InitManager();

            // Act
            var response = await SaveAsync();
            
            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task DeleteSection_DeleteFromRedis_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await InitManager();
            await AddTextSectionAsync(11, 1);

            // Act
            var response = await DeleteValueAsync(11, 1);
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
            await AddTextSectionAsync(11, 1);
            await SaveAsync();
            
            // Act
            var response = await DeleteValueAsync(11, 1);
            var content = await response.Content.ReadAsStringAsync();
            
            await SaveAsync();

            var isExistContent = await _redis.KeyExistsAsync($"content:admin:11:1");
            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.False(isExistContent);
        }
        
        [Fact]
        public async Task GetSectionTransferInfo_ReturnOk_ReturnsOkResult()
        {
            // Arrange
            await InitManager();
            int number = 1;
            int flow = 1;

            // Act
            var response = await _client.GetAsync($"/api/Creator/TransferMenu?bookId=1");
            var content = await response.Content.ReadAsStringAsync();
            var allContent = JsonConvert.DeserializeObject<TransferInfoDTO>(content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(allContent);
            Assert.True(allContent.Chapters.Count > 0);
            Assert.True(allContent.Chapters.Count > 0);
            Assert.True(allContent.Chapters.Count > 0);
        }

        private async Task<HttpResponseMessage> AddChoiceAsync(int number, int flow, int choiceNumber, string text
            , int nextNumber = 1, int nextFlow = 1)
        {
            var response = await _client
                .PostAsync(
                    $"/api/Creator/AddChoice?" +
                    $"number={number}&flow={flow}&choiceNumber={choiceNumber}" +
                    $"&nextNumber={nextNumber}&nextFlow={nextFlow}&text={text}",
                    null);
            return response;
        }
        
        private async Task<HttpResponseMessage> UpdateChoiceAsync(int number, int flow, int choiceNumber, string text)
        {
            var response = await _client
                .PutAsync(
                    $"/api/Creator/UpdateChoice?" +
                    $"number={number}&flow={flow}&choiceNumber={choiceNumber}&nextNumber=1&nextFlow=1&text={text}",
                    null);
            return response;
        }
        
        private async Task<HttpResponseMessage> DeleteChoiceAsync(int number, int flow, int choiceNumber)
        {
            var response = await _client
                .DeleteAsync(
                    $"/api/Creator/DeleteChoice?" +
                    $"number={number}&flow={flow}&choiceNumber={choiceNumber}&nextNumber=1&nextFlow=1");
            return response;
        }
        
        [Fact]
        public async Task AddChoice_AddingValueToRedis_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await InitManager();

            const int choiceNumber = 3;
            const int number = 10;
            const int flow = 1;
            const string text = "Тестовая ссылка на 1 секцию из 10";
            
            // Act
            var response = await AddChoiceAsync(number, flow, choiceNumber, text);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.True(response.IsSuccessStatusCode);

            var redisValues = await _redis.HashGetAllAsync($"choiceManager:admin");
            Assert.True(redisValues.Length == 1);
            var redisValue = redisValues.FirstOrDefault();
            Assert.Equal(redisValue.Name, $"{choiceNumber}:{number}:{flow}");

            Assert.True(redisValue.Value.HasValue, "redisValue.Value != null");
            var choiceContent = JsonConvert.DeserializeObject<ChoiceContent>(redisValue.Value!)!;
            
            Assert.Equal(1, choiceContent.NextNumber);
            Assert.Equal(1, choiceContent.NextFlow);
            Assert.Equal(text, choiceContent.Content);
            Assert.Equal(ChangeType.Create, choiceContent.Operation);
        }
        
        [Fact]
        public async Task AddChoice_SaveValueToDb_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await InitManager();

            const int choiceNumber = 3;
            const int number = 10;
            const int flow = 1;
            const string text = "Тестовая ссылка на 1 секцию из 10";
            
            await DeleteChoiceAsync(number, flow, choiceNumber);
            await SaveAsync();
            
            // Act
            var response = await AddChoiceAsync(number, flow, choiceNumber, text);
            var content = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode);

            var saveResponse = await SaveAsync();
            
            // Assert
            Assert.True(saveResponse.IsSuccessStatusCode);
            var redisValues = await _redis.HashGetAllAsync($"choiceManager:admin");
            Assert.True(redisValues.Length == 0);

            
            // After
            await DeleteChoiceAsync(number, flow, choiceNumber);
            await SaveAsync();
        }
        
        [Fact]
        public async Task UpdateChoiceToManager_ChangeValueToRedis_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await InitManager();

            const int choiceNumber = 3;
            const int number = 10;
            const int flow = 1;
            const string text = "Обневленная тестовая ссылка на 1 секцию из 10";

            await AddChoiceAsync(number, flow, choiceNumber, "начальный текст для выбора, который скоро удалится");
            await SaveAsync();
            
            // Act
            var response = await UpdateChoiceAsync(number, flow, choiceNumber, text);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.True(response.IsSuccessStatusCode);

            var redisValues = await _redis.HashGetAllAsync($"choiceManager:admin");
            Assert.True(redisValues.Length == 1);
            var redisValue = redisValues.FirstOrDefault();
            Assert.Equal(redisValue.Name, $"{choiceNumber}:{number}:{flow}");

            Assert.True(redisValue.Value.HasValue, "redisValue.Value != null");
            var choiceContent = JsonConvert.DeserializeObject<ChoiceContent>(redisValue.Value!)!;
            
            Assert.Equal(1, choiceContent.NextNumber);
            Assert.Equal(1, choiceContent.NextFlow);
            Assert.Equal(text, choiceContent.Content);
            Assert.Equal(ChangeType.Update, choiceContent.Operation);

            await DeleteChoiceAsync(number, flow, choiceNumber);
            await SaveAsync();
        }
        
        [Fact]
        public async Task UpdateChoiceToManager_ChangeValueInDb_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await InitManager();

            const int choiceNumber = 3;
            const int number = 10;
            const int flow = 1;
            const string text = "Обневленная тестовая ссылка на 1 секцию из 10";

            await AddChoiceAsync(number, flow, choiceNumber, "начальный текст для выбора, который скоро удалится");
            await SaveAsync();
            
            // Act
            var response = await UpdateChoiceAsync(number, flow, choiceNumber, text);
            var content = await response.Content.ReadAsStringAsync();

            var saveResponse = await SaveAsync();
            
            // Assert
            Assert.True(response.IsSuccessStatusCode);
            var redisValues = await _redis.HashGetAllAsync($"choiceManager:admin");
            Assert.True(redisValues.Length == 0);
            
            await DeleteChoiceAsync(number, flow, choiceNumber);
            await SaveAsync();
;        }

        [Fact]
        public async Task DeleteChoiceToManager_AddDeleteChoiceToToRedis_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await InitManager();

            const int choiceNumber = 3;
            const int number = 10;
            const int flow = 1;
            
            await AddChoiceAsync(number, flow, choiceNumber, "начальный текст для выбора, который скоро удалится");
            await SaveAsync();
            
            // Act
            var response = await DeleteChoiceAsync(number, flow, choiceNumber);
            var content = await response.Content.ReadAsStringAsync();
            
            // Assert
            Assert.True(response.IsSuccessStatusCode);

            var redisValues = await _redis.HashGetAllAsync($"choiceManager:admin");
            Assert.True(redisValues.Length == 1);
            var redisValue = redisValues.FirstOrDefault();
            Assert.Equal(redisValue.Name, $"{choiceNumber}:{number}:{flow}");

            Assert.True(redisValue.Value.HasValue, "redisValue.Value != null");
            var choiceContent = JsonConvert.DeserializeObject<ChoiceContent>(redisValue.Value!)!;
            
            Assert.Equal(0, choiceContent.NextNumber);
            Assert.Equal(0, choiceContent.NextFlow);
            Assert.Null(choiceContent.Content);
            Assert.Equal(ChangeType.Delete, choiceContent.Operation);
        }
        
        [Fact]
        public async Task DeleteChoiceToManager_DeleteValueFromDb_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await InitManager();

            const int choiceNumber = 3;
            const int number = 10;
            const int flow = 1;

            await AddChoiceAsync(number, flow, choiceNumber, "начальный текст для выбора, который скоро удалится");
            await SaveAsync();
            
            // Act
            var response = await DeleteChoiceAsync(number, flow, choiceNumber);
            var content = await response.Content.ReadAsStringAsync();

            var saveResponse = await SaveAsync();
            
            // Assert
            Assert.True(response.IsSuccessStatusCode);
            var redisValues = await _redis.HashGetAllAsync($"choiceManager:admin");
            Assert.True(redisValues.Length == 0);
        }
        
                
        [Fact]
        public async Task AddChoice_ErrorWithNoInitManager_ReturnsBadRequest()
        {
            // Arrange
            await ClearTable();
            // await InitManager();

            const int choiceNumber = 3;
            const int number = 10;
            const int flow = 1;
            const string text = "Тестовая ссылка на 1 секцию из 10";
            
            // Act
            var response = await AddChoiceAsync(number, flow, choiceNumber, text);
            var content = await response.Content.ReadAsStringAsync();

            await SaveAsync();
            
            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
            var redisValues = await _redis.HashGetAllAsync($"choiceManager:admin");
            Assert.True(redisValues.Length == 0);
        }

        [Fact]
        public async Task AddChoice_AddManyChoicesAndSave_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await InitManager();
        
            const int choiceNumber = 3;
            const int startNumber = 1;
            const int endNumber = 10;
            const int flow = 1;
            const string text = "Тестовая ссылка на 1 секцию из итерации";
            
            // Act // Assert
            for (var i = startNumber; i <= endNumber; i++)
            {
                var response = await AddChoiceAsync(i, flow, choiceNumber, text);
                var content = await response.Content.ReadAsStringAsync(); 
                Assert.True(response.StatusCode == HttpStatusCode.OK);
            }
            
            var redisValues = await _redis.HashGetAllAsync($"choiceManager:admin");
            Assert.True(redisValues.Length == 10);
            
            var saveResponse = await SaveAsync();
            Assert.True(saveResponse.StatusCode == HttpStatusCode.OK);

            // After
            for (var i = startNumber; i <= endNumber; i++)
            {
                var response = await DeleteChoiceAsync(i, flow, choiceNumber);
            }
            
            await SaveAsync();
        }

        [Fact]
        public async Task UpdateChoice_AddAndUpdateManyChoices_ReturnsBOkResult()
        {
            // Arrange
            await ClearTable();
            await InitManager();
        
            const int choiceNumber = 3;
            const int startNumber = 1;
            const int endNumber = 10;
            const int flow = 1;
            const string text = "Тестовая ссылка на 1 секцию из итерации";
            
            // Act // Assert
            for (var i = startNumber; i <= endNumber; i++)
            {
                var response = await AddChoiceAsync(i, flow, choiceNumber, text);
                var content = await response.Content.ReadAsStringAsync(); 
                Assert.True(response.StatusCode == HttpStatusCode.OK);
            }
            
            var redisValues = await _redis.HashGetAllAsync($"choiceManager:admin");
            Assert.True(redisValues.Length == 10);
            
            // Act // Assert
            for (var i = startNumber; i <= endNumber; i++)
            {
                var response = await UpdateChoiceAsync(i, flow, choiceNumber, "Обновка");
                var content = await response.Content.ReadAsStringAsync(); 
                Assert.True(response.StatusCode == HttpStatusCode.OK);
            }
            
            var redisValuesUpdate = await _redis.HashGetAllAsync($"choiceManager:admin");
            Assert.True(redisValuesUpdate.Length == 10);
            
            var saveResponse = await SaveAsync();
            var saveContent = saveResponse.Content.ReadAsStringAsync();
            Assert.True(saveResponse.StatusCode == HttpStatusCode.OK);

            // After
            for (var i = startNumber; i <= endNumber; i++)
            {
                var response = await DeleteChoiceAsync(i, flow, choiceNumber);
            }
            
            await SaveAsync();
        }
        
        [Fact]
        public async Task UpdateChoice_AddUpdateManyChoicesAndSave_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await InitManager();
        
            const int choiceNumber = 3;
            const int startNumber = 1;
            const int endNumber = 10;
            const int flow = 1;
            const string text = "Тестовая ссылка на 1 секцию из итерации";
            
            // Act // Assert
            for (var i = startNumber; i <= endNumber; i++)
            {
                var response = await AddChoiceAsync(i, flow, choiceNumber, text);
                var content = await response.Content.ReadAsStringAsync(); 
                Assert.True(response.StatusCode == HttpStatusCode.OK);
            }
            
            var redisValues = await _redis.HashGetAllAsync($"choiceManager:admin");
            Assert.True(redisValues.Length == 10);
            
            var saveResponse1 = await SaveAsync();
            var saveContent1 = saveResponse1.Content.ReadAsStringAsync();
            Assert.True(saveResponse1.StatusCode == HttpStatusCode.OK);
            
            // Act // Assert
            for (var i = startNumber; i <= endNumber; i++)
            {
                var response = await UpdateChoiceAsync(i, flow, choiceNumber, "Обновка");
                var content = await response.Content.ReadAsStringAsync(); 
                Assert.True(response.StatusCode == HttpStatusCode.OK);
            }
            
            var redisValuesUpdate = await _redis.HashGetAllAsync($"choiceManager:admin");
            Assert.True(redisValuesUpdate.Length == 10);
            
            var saveResponse2 = await SaveAsync();
            var saveContent2 = saveResponse2.Content.ReadAsStringAsync();
            Assert.True(saveResponse2.StatusCode == HttpStatusCode.OK);

            // After
            for (var i = startNumber; i <= endNumber; i++)
            {
                var response = await DeleteChoiceAsync(i, flow, choiceNumber);
            }
            
            await SaveAsync();
        }
        
        [Fact]
        public async Task SectionOperationsExecute_AddThreeInRedisAndDelete_ReturnsBadRequestResult()
        {
            // Arrange
            await ClearTable();
            await InitManager();

            var request1 = await AddTextSectionAsync(11, 1);
            Assert.True(request1.StatusCode == HttpStatusCode.OK);
            var request2 = await AddTextSectionAsync(12, 1);
            Assert.True(request2.StatusCode == HttpStatusCode.OK);
            var request3 = await AddImageSectionAsync(12, 2);
            var content = request3.Content.ReadAsStringAsync();
            Assert.True(request3.StatusCode == HttpStatusCode.OK);
            
            // var request6 = await SaveAsync();
            // Assert.True(request6.StatusCode == HttpStatusCode.OK);

            var request8 = await DeleteValueAsync(12, 2);
            Assert.True(request8.StatusCode == HttpStatusCode.OK);
           
            var request14 = await DeleteValueAsync(12, 1);
            Assert.True(request14.StatusCode == HttpStatusCode.OK);

            var request15 = await DeleteValueAsync(11, 1);
            Assert.True(request15.StatusCode == HttpStatusCode.OK);
            
            var request16 = await SaveAsync();
            var content3 = await request16.Content.ReadAsStringAsync();
            Assert.True(request16.StatusCode == HttpStatusCode.BadRequest);
            Assert.Equal("Nothing to save", content3);
        }
        
        [Fact]
        public async Task SectionOperationsExecute_AddThreeAndDeleteInDb_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await InitManager();

            var request1 = await AddTextSectionAsync(11, 1);
            Assert.True(request1.StatusCode == HttpStatusCode.OK);
            var request2 = await AddTextSectionAsync(12, 1);
            Assert.True(request2.StatusCode == HttpStatusCode.OK);
            var request3 = await AddTextSectionAsync(12, 2);
            var content = request3.Content.ReadAsStringAsync();
            Assert.True(request3.StatusCode == HttpStatusCode.OK);
            
            var request6 = await SaveAsync();
            Assert.True(request6.StatusCode == HttpStatusCode.OK);

            var request8 = await DeleteValueAsync(12, 2);
            Assert.True(request8.StatusCode == HttpStatusCode.OK);
           
            var request14 = await DeleteValueAsync(12, 1);
            Assert.True(request14.StatusCode == HttpStatusCode.OK);

            var request15 = await DeleteValueAsync(11, 1);
            Assert.True(request15.StatusCode == HttpStatusCode.OK);
            
            var request16 = await SaveAsync();
            var content3 = await request16.Content.ReadAsStringAsync();
            Assert.True(request16.StatusCode == HttpStatusCode.OK);
        }
        
        [Fact]
        public async Task SectionOperationsExecute_AddThreeUpdateAndDeleteInDb_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await InitManager();

            var request1 = await AddTextSectionAsync(11, 1);
            Assert.True(request1.StatusCode == HttpStatusCode.OK);
            var request2 = await AddTextSectionAsync(12, 1);
            Assert.True(request2.StatusCode == HttpStatusCode.OK);
            var request3 = await AddTextSectionAsync(12, 2);
            var content = request3.Content.ReadAsStringAsync();
            Assert.True(request3.StatusCode == HttpStatusCode.OK);
            
            var request17 = await UpdateTextSectionAsync(11, 1);
            Assert.True(request17.StatusCode == HttpStatusCode.OK);
            
            var request6 = await SaveAsync();
            Assert.True(request6.StatusCode == HttpStatusCode.OK);

            var request11 = await UpdateTextSectionAsync(11, 1);
            Assert.True(request11.StatusCode == HttpStatusCode.OK);
            
            var request8 = await DeleteValueAsync(12, 2);
            Assert.True(request8.StatusCode == HttpStatusCode.OK);
           
            var request14 = await DeleteValueAsync(12, 1);
            Assert.True(request14.StatusCode == HttpStatusCode.OK);

            var request15 = await DeleteValueAsync(11, 1);
            Assert.True(request15.StatusCode == HttpStatusCode.OK);
            
            var request16 = await SaveAsync();
            var content3 = await request16.Content.ReadAsStringAsync();
            Assert.True(request16.StatusCode == HttpStatusCode.OK);
        }
        
        [Fact]
        public async Task SectionOperationsExecute_AddThreeSectionsAndAddTwoChoicesAndSaveAndDeleteAllAndSave_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await InitManager();

            var request1 = await AddTextSectionAsync(11, 1);
            Assert.True(request1.StatusCode == HttpStatusCode.OK);
            var request2 = await AddTextSectionAsync(12, 1);
            Assert.True(request2.StatusCode == HttpStatusCode.OK);
            var request3 = await AddTextSectionAsync(12, 2);
            Assert.True(request3.StatusCode == HttpStatusCode.OK);
            
            var requestChoice1 = await AddChoiceAsync(11, 1, 1, "first", 12, 1);
            Assert.True(requestChoice1.StatusCode == HttpStatusCode.OK);

            var requestChoice12 = await AddChoiceAsync(11, 1, 2, "second", 12, 1);
            Assert.True(requestChoice12.StatusCode == HttpStatusCode.OK);
            
            var request6 = await SaveAsync();
            Assert.True(request6.StatusCode == HttpStatusCode.OK);
            
            var request8 = await DeleteValueAsync(12, 2);
            Assert.True(request8.StatusCode == HttpStatusCode.OK);
           
            var request14 = await DeleteValueAsync(12, 1);
            Assert.True(request14.StatusCode == HttpStatusCode.OK);

            var request15 = await DeleteValueAsync(11, 1);
            Assert.True(request15.StatusCode == HttpStatusCode.OK);
            
            var request16 = await SaveAsync();
            var content3 = await request16.Content.ReadAsStringAsync();
            Assert.True(request16.StatusCode == HttpStatusCode.OK);
        }
        
        [Fact]
        public async Task SectionOperationsExecute_AddThreeSections_Save_AddTwoChoices_Save_DeleteAll_Save_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await InitManager();

            var request1 = await AddTextSectionAsync(11, 1);
            Assert.True(request1.StatusCode == HttpStatusCode.OK);
            var request2 = await AddTextSectionAsync(12, 1);
            Assert.True(request2.StatusCode == HttpStatusCode.OK);
            var request3 = await AddTextSectionAsync(12, 2);
            Assert.True(request3.StatusCode == HttpStatusCode.OK);
            
            var request21 = await SaveAsync();
            Assert.True(request21.StatusCode == HttpStatusCode.OK);
            
            var requestChoice1 = await AddChoiceAsync(11, 1, 1, "first", 12, 1);
            Assert.True(requestChoice1.StatusCode == HttpStatusCode.OK);

            var requestChoice12 = await AddChoiceAsync(11, 1, 2, "second", 12, 1);
            Assert.True(requestChoice12.StatusCode == HttpStatusCode.OK);
            
            var request6 = await SaveAsync();
            Assert.True(request6.StatusCode == HttpStatusCode.OK);
            
            var request8 = await DeleteValueAsync(12, 2);
            Assert.True(request8.StatusCode == HttpStatusCode.OK);
           
            var request14 = await DeleteValueAsync(12, 1);
            Assert.True(request14.StatusCode == HttpStatusCode.OK);

            var request15 = await DeleteValueAsync(11, 1);
            Assert.True(request15.StatusCode == HttpStatusCode.OK);
            
            var request16 = await SaveAsync();
            var content3 = await request16.Content.ReadAsStringAsync();
            Assert.True(request16.StatusCode == HttpStatusCode.OK);
        }
        
        [Fact]
        public async Task SectionOperationsExecute_AddThreeSection_AddTwoChoices_Save_DeleteChoices_DeleteValues_Save_ReturnsOkResult()
        {
            // Arrange
            await ClearTable();
            await InitManager();
        
            var request1 = await AddTextSectionAsync(11, 1);
            Assert.True(request1.StatusCode == HttpStatusCode.OK);
            var request2 = await AddTextSectionAsync(12, 1);
            Assert.True(request2.StatusCode == HttpStatusCode.OK);
            var request3 = await AddImageSectionAsync(12, 2);
            var content = request3.Content.ReadAsStringAsync();
            Assert.True(request3.StatusCode == HttpStatusCode.OK);
            var request4 = 
                await AddChoiceAsync(11, 1, 1, "первый", 12, 1);
            Assert.True(request4.StatusCode == HttpStatusCode.OK);
            var request5 = 
                await AddChoiceAsync(11, 1, 2, "второй", 12, 1);
            Assert.True(request5.StatusCode == HttpStatusCode.OK);
        
            var request6 = await SaveAsync();
            Assert.True(request6.StatusCode == HttpStatusCode.OK);
        
            var request8 = await DeleteChoiceAsync(11, 1, 1);
            Assert.True(request8.StatusCode == HttpStatusCode.OK);
           
            var request14 = await DeleteChoiceAsync(11, 1, 2);
            Assert.True(request14.StatusCode == HttpStatusCode.OK);
        
            var request10 = await DeleteValueAsync(12, 2);
            Assert.True(request10.StatusCode == HttpStatusCode.OK);
            
            var request9 = await DeleteValueAsync(12, 1);
            Assert.True(request9.StatusCode == HttpStatusCode.OK);
            //
            var request15 = await DeleteValueAsync(11, 1);
            Assert.True(request15.StatusCode == HttpStatusCode.OK);
            
            var request12 = await SaveAsync();
            var lastContent2 = await request12.Content.ReadAsStringAsync();
            Assert.True(HttpStatusCode.OK == request12.StatusCode, lastContent2);
        }
        
    }
}
