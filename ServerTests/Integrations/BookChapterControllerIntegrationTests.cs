using AuthorVerseServer.DTO;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace ServerTests.Integrations
{
    public class BookChapterControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CreateJWTtokenService _token;

        public BookChapterControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            var configuration = factory.Services.GetRequiredService<IConfiguration>();

            _token = new CreateJWTtokenService(configuration);
            _client = factory.CreateClient();

            //string jwtToken = _token.GenerateJwtToken("admin");
            //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        [Fact]
        public async Task GetBookChapter_NotAuthorize_ReturnsOkResult()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("/api/BookChapter/Read?bookId=1");
            var content = await response.Content.ReadAsStringAsync();
            var pageChapters = JsonConvert.DeserializeObject<PageBookChaptersDTO>(content);
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(pageChapters);
            Assert.True(pageChapters.LastReadingNumber == 0);
        }

        [Fact]
        public async Task GetBookChapter_Authorize_ReturnsOkResult()
        {
            // Arrange
            string jwtToken = _token.GenerateJwtToken("admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            // Act
            var response = await _client.GetAsync("/api/BookChapter/Read?bookId=1");
            var content = await response.Content.ReadAsStringAsync();
            var pageChapters = JsonConvert.DeserializeObject<PageBookChaptersDTO>(content);
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(pageChapters);
            Assert.True(pageChapters.LastReadingNumber != 0);
        }

        // должен быть включен сервер
        [Fact]
        public async Task PublicateChapter_Ok_ReturnsOkResult()
        {
            // Arrange
            string jwtToken = _token.GenerateJwtToken("admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            // Act
            var response = await _client.PostAsync("/api/BookChapter/Publicate/1", null);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAuthorChaptersAsync_Ok_ReturnsOkResult()
        {
            // Arrange
            string jwtToken = _token.GenerateJwtToken("admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            // Act
            var response = await _client.PostAsync("/api/BookChapter/AuthorChapters?bookId=1", null);
            var content = await response.Content.ReadAsStringAsync();

            var chapters = JsonConvert.DeserializeObject<ICollection<ShortAuthorChapterDTO>>(content);
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(chapters);

            foreach (var chapter in chapters)
            {
                Assert.True(chapter.Number != 0);
                Assert.True(chapter.BookChapterId != 0);
            }
        }


        [Fact]
        public async Task GetAuthorDetaildChapterAsync_Ok_ReturnsOkResult()
        {
            // Arrange
            string jwtToken = _token.GenerateJwtToken("admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            // Act
            var response = await _client.PostAsync("/api/BookChapter/DetailChapter?bookId=1", null);
            var content = await response.Content.ReadAsStringAsync();

            var chapter = JsonConvert.DeserializeObject<DetaildAuthorChapterDTO>(content);
            
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(chapter);
            Assert.False(string.IsNullOrEmpty(chapter.Description));
            Assert.NotNull(chapter.Characters);
        }

        [Fact]
        public async Task UpdateChapter_Ok_ReturnsOkResult()
        {
            // Arrange
            string jwtToken = _token.GenerateJwtToken("admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            // Act
            var response = await _client.PutAsync("/api/BookChapter?chapterId=1&place=TestPlace&description=TestDescription", null);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AddNewChapter_Ok_ReturnsOkResult()
        {
            // Arrange
            string jwtToken = _token.GenerateJwtToken("admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            // Act
            var response = await _client.PostAsync("/api/BookChapter?title=newTest&lastChapterId=1", null);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(int.TryParse(content, out int chapterId));
            Assert.True(chapterId > 0);

            var deleteResponse = await _client.DeleteAsync($"/api/BookChapter?chapterId={content}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteChapter_NotLastChapter_ReturnsBadRequest()
        {
            // Arrange
            string jwtToken = _token.GenerateJwtToken("admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            // Act
            var response = await _client.DeleteAsync($"/api/BookChapter?chapterId={1}");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
