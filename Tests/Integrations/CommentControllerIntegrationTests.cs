using AuthorVerseServer.DTO;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Xunit;
using AuthorVerseServer.Services;

namespace AuthorVerseServer.Tests.Integrations
{
    public class CommentControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CreateJWTtokenService _token;
        public CommentControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            var currentDirectory = Environment.CurrentDirectory;
            string path = Path.Combine(currentDirectory, "../../../");
            factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseSolutionRelativeContentRoot(path);
            });

            _client = factory.CreateClient();

            var configuration = factory.Services.GetRequiredService<IConfiguration>();

            _token = new CreateJWTtokenService(configuration);
        }

        [Fact]
        public async Task CreateComment_CheckModelStateWork_ReturnsBadResult()
        {
            // Arrange
            var bookDTO = new CreateCommentDTO
            {
                UserId = "admin",
                BookId = 0,
                Text = "Слишком коротко",
            };

            string jwtToken = _token.GenerateJwtToken("admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var requestContent = new StringContent(JsonConvert.SerializeObject(bookDTO), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/Comment/Create", requestContent);

            // Act
            //var response = await _client.PostAsJsonAsync("/api/Comment/Create", bookDTO);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateComment_ReturnsOkResult()
        {
            // Arrange
            Random random = new Random();

            var bookDTO = new CreateCommentDTO
            {
                UserId = "admin",
                BookId = random.Next(1, 1000),
                Text = "Я и мой комментарий. Почти как Мама, папа я и бд, но только с комментарием, длинна которого должна быть ",
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Comment/Create", bookDTO);

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
