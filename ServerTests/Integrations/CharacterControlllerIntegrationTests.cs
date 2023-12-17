using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Server.Tests.Integrations
{
    public class CharacterControlllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public CharacterControlllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();

            var configuration = factory.Services.GetRequiredService<IConfiguration>();

            var token = new CreateJWTtokenService(configuration);

            string jwtToken = token.GenerateJwtToken("admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        [Fact]
        public async Task GetBookCharacter_Ok_ReturnsOkResult()
        {
            // Act
            int bookId = 1;
            var response = await _client.GetAsync($"/api/Character/{bookId}");

            var content = await response.Content.ReadAsStringAsync();
            var characters = JsonConvert.DeserializeObject<ICollection<BookCharacterDTO>>(content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(characters);
            foreach (var character in characters)
            {
                Assert.NotNull(character);
                Assert.False(string.IsNullOrEmpty(character.Name));
            }
        }

        [Fact]
        public async Task GetBookCharacterByName_Ok_ReturnsOkResult()
        {
            // Act
            string searchName = "Анджелина";
            int bookId = 1;
            var response = await _client.GetAsync($"/api/Character/{bookId}/{searchName}");

            var content = await response.Content.ReadAsStringAsync();
            var characters = JsonConvert.DeserializeObject<ICollection<BookCharacterDTO>>(content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(characters);
            foreach (var character in characters)
            {
                Assert.NotNull(character);
                Assert.False(string.IsNullOrEmpty(character.Name));
                Assert.Contains(searchName, character.Name);
            }
        }

        [Fact]
        public async Task AddCharacterToBook_NameIsAlreadyExist_ReturnsBadRequest()
        {
            // Act
            string name = "Анджелина";
            int bookId = 1;
            var response = await _client.PostAsync($"/api/Character/Book/{bookId}/{name}", null);

            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task AddCharacterToBook_Ok_ReturnsOkResult()
        {
            // Act
            string uniqueId = Guid.NewGuid().ToString();
            string name = $"Yuri-{uniqueId}";
            int bookId = 1;
            var response = await _client.PostAsync($"/api/Character/Book/{bookId}/{name}", null);

            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(int.TryParse(content, out int chapterId));
            Assert.True(chapterId > 0);
        }


        [Fact]
        public async Task AddCharacterToChapter_AlreadyExist_ReturnsBadRequest()
        {
            // Act
            int chapterId = 1;
            int characterId = 1;
            var response = await _client.PostAsync($"/api/Character/Chapter/{chapterId}/{characterId}", null);

            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        public async Task<int> AddCharacterToChapterAndBook()
        {
            var responseAct = await _client.PostAsync($"/api/Character/Book/{1}/{$"Yuri-{Guid.NewGuid()}"}", null);
            int newCharactedId = int.Parse(await responseAct.Content.ReadAsStringAsync());

            int chapterId = 1;
            var response = await _client.PostAsync($"/api/Character/Chapter/{chapterId}/{newCharactedId}", null);

            return newCharactedId;
        }

        [Fact]
        public async Task AddCharacterToChapter_Ok_ReturnsOkResult()
        {
            // Act
            var responseAct = await _client.PostAsync($"/api/Character/Book/{1}/{$"Yuri-{Guid.NewGuid()}"}", null);
            int newCharactedId = int.Parse(await responseAct.Content.ReadAsStringAsync());

            int chapterId = 1;
            var response = await _client.PostAsync($"/api/Character/Chapter/{chapterId}/{newCharactedId}", null);

            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task UpdateCharacter_Ok_ReturnsOkResult()
        {
            // Act
            var chapter = new UpdateCharacterDTO
            {
                CharacterId = 1,
                Description = "Test",
                Name = $"Test-{Guid.NewGuid()}",
            };

            var response = await _client.PutAsJsonAsync($"/api/Character/Text", chapter);

            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task DeleteCharacterFromChapter_Ok_ReturnsOkResult()
        {
            // Act
            int characterId = await AddCharacterToChapterAndBook();

            var response = await _client.DeleteAsync($"/api/Character/{1}/{characterId}");

            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteCharacterFromBookOk_ReturnsOkResult()
        {
            // Act
            int characterId = await AddCharacterToChapterAndBook();

            var response = await _client.DeleteAsync($"/api/Character/{characterId}");

            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
