using AuthorVerseServer.DTO;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;

namespace ServerTests.Integrations
{
    public class ChapterSectionControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        // private readonly CreateJWTtokenService _token;

        public ChapterSectionControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            var configuration = factory.Services.GetRequiredService<IConfiguration>();

            var token = new CreateJWTtokenService(configuration);
            _client = factory.CreateClient();

            // var jwtToken = token.GenerateJwtToken("admin");
            // _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        [Fact]
        public async Task GetBookChapter_GetManagerContent_ReturnsOkResult()
        {
            // Arrange
            int chapterId = 1;
            int flow = 1;
            // Act
            var response = await _client.GetAsync($"/api/ChapterSection/GetManagerBy?chapterId={chapterId}&flow={flow}");
            var content = await response.Content.ReadAsStringAsync();
            var manager = JsonConvert.DeserializeObject<ContentManagerDTO> (content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(manager);
            Assert.True(manager.ContentsDTO.Count > 1);
        }

        [Fact]
        public async Task GetBookChapter_GetSecondChoiceManagerContent_ReturnsOkResult()
        {
            // Arrange
            int chapterId = 1;
            int flow = 1;
            int lastChoiceNumber = 5;

            // Act
            var response = await _client.GetAsync($"/api/ChapterSection/GetManagerBy?chapterId={chapterId}&flow={flow}&lastChoiceNumber={lastChoiceNumber}");
            var content = await response.Content.ReadAsStringAsync();
            var manager = JsonConvert.DeserializeObject<ContentManagerDTO>(content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(manager);
            Assert.True(manager.ContentsDTO.Count > 0);
        }

        [Fact]
        public async Task GetBookChapter_GetThirdManagerContent_ReturnsOkResult()
        {
            // Arrange
            int chapterId = 1;
            int flow = 1;
            int lastChoiceNumber = 8;

            // Act
            var response = await _client.GetAsync($"/api/ChapterSection/GetManagerBy?chapterId={chapterId}&flow={flow}&lastChoiceNumber={lastChoiceNumber}");
            var content = await response.Content.ReadAsStringAsync();
            var manager = JsonConvert.DeserializeObject<ContentManagerDTO>(content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(manager);
            Assert.True(manager.ContentsDTO.Count > 0);
        }

        [Fact]
        public async Task GetBookChapter_GetFinalManagerContent_ReturnsOkResult()
        {
            // Arrange
            int chapterId = 1;
            int flow = 2;
            int lastChoiceNumber = 5;

            // Act
            var response = await _client.GetAsync($"/api/ChapterSection/GetManagerBy?chapterId={chapterId}&flow={flow}&lastChoiceNumber={lastChoiceNumber}");
            var content = await response.Content.ReadAsStringAsync();
            var manager = JsonConvert.DeserializeObject<ContentManagerDTO>(content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(manager);
            Assert.True(manager.ContentsDTO.Count > 0);
            Assert.Null(manager.Choice);
        }


        [Fact]
        public async Task GetTextContentSections_CheckOnExistAndCorrect_ReturnsOkResult()
        {
            // Arrange
            int contentId = 3;

            // Act
            var response = await _client.GetAsync($"/api/ChapterSection/GetTextContentBy?contentId={contentId}");
            var content = await response.Content.ReadAsStringAsync();
            var textContent = JsonConvert.DeserializeObject<SectionDTO>(content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(textContent);
            Assert.False(string.IsNullOrEmpty(textContent.Content));
        }

        [Fact]
        public async Task GetImageContentSections_CheckOnExistAndCorrect_ReturnsOkResult()
        {
            // Arrange
            int contentId = 2;

            // Act
            var response = await _client.GetAsync($"/api/ChapterSection/GetImageContentBy?contentId={contentId}");
            var content = await response.Content.ReadAsStringAsync();
            var textContent = JsonConvert.DeserializeObject<SectionDTO>(content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(textContent);
            Assert.False(string.IsNullOrEmpty(textContent.Content));
        }

        [Fact]
        public async Task GetAudioContentSections_CheckOnExistAndCorrect_ReturnsOkResult()
        {
            // Arrange
            int contentId = 1;

            // Act
            var response = await _client.GetAsync($"/api/ChapterSection/GetAudioContentBy?contentId={contentId}");
            var content = await response.Content.ReadAsStringAsync();
            var textContent = JsonConvert.DeserializeObject<SectionDTO>(content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(textContent);
            Assert.False(string.IsNullOrEmpty(textContent.Content));
        }


        [Fact]
        public async Task GetAutoDetectedTypeContentSections_ShouldBeMatchWithAudioMethod_ReturnsOkResult()
        {
            // Arrange
            int contentId = 1;

            // Act
            var response = await _client.GetAsync($"/api/ChapterSection/GetAudioContentBy?contentId={contentId}");
            var content = await response.Content.ReadAsStringAsync();

            var autoResponse = await _client.GetAsync($"/api/ChapterSection/GetAutoTypeContentBy?contentId={contentId}&type=1");
            var autoContent = await autoResponse.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(HttpStatusCode.OK, autoResponse.StatusCode);
            Assert.Equal(autoContent, content);
        }


        [Fact]
        public async Task GetAutoDetectedTypeContentSections_ShouldBeMatchWithImageMethod_ReturnsOkResult()
        {
            // Arrange
            int contentId = 2;

            // Act
            var response = await _client.GetAsync($"/api/ChapterSection/GetImageContentBy?contentId={contentId}");
            var content = await response.Content.ReadAsStringAsync();

            var autoResponse = await _client.GetAsync($"/api/ChapterSection/GetAutoTypeContentBy?contentId={contentId}&type=3");
            var autoContent = await autoResponse.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(HttpStatusCode.OK, autoResponse.StatusCode);
            Assert.Equal(autoContent, content);
        }

        [Fact]
        public async Task GetAutoDetectedTypeContentSections_ShouldBeMatchWithTextMethod_ReturnsOkResult()
        {
            // Arrange
            int contentId = 3;

            // Act
            var response = await _client.GetAsync($"/api/ChapterSection/GetTextContentBy?contentId={contentId}");
            var content = await response.Content.ReadAsStringAsync();

            var autoResponse = await _client.GetAsync($"/api/ChapterSection/GetAutoTypeContentBy?contentId={contentId}&type=0");
            var autoContent = await autoResponse.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(HttpStatusCode.OK, autoResponse.StatusCode);
            Assert.Equal(autoContent, content);
        }

        [Fact]
        public async Task GetAllContentSections_ShouldReturnPathOfContentIncludingChoice_ReturnsOkResult()
        {
            // Arrange
            int chapterId = 1;
            int flow = 1;

            // Act
            var response = await _client.GetAsync($"/api/ChapterSection/GetAllContentBy?chapterId={chapterId}&flow={flow}");
            var content = await response.Content.ReadAsStringAsync();
            var allContent = JsonConvert.DeserializeObject<AllContentDTO>(content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(allContent);
            Assert.True(allContent.SectionsDTO.Count > 1);
            Assert.NotNull(allContent.Choice);
        }


        [Fact]
        public async Task GetAllWithModelContentSections_ShouldReturnPathOfContentIncludingChoice_ReturnsOkResult()
        {
            // Arrange
            int chapterId = 1;
            int flow = 1;

            // Act
            var response = await _client.GetAsync($"/api/ChapterSection/GetAllWithModelContentBy?chapterId={chapterId}&flow={flow}");
            var content = await response.Content.ReadAsStringAsync();
            var allContent = JsonConvert.DeserializeObject<AllContentWithModelDTO>(content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(allContent);
            Assert.True(allContent.ContentsDTO.Count > 0);
            Assert.NotNull(allContent.Choice);
        }
        

    }
}
