using AuthorVerseServer.DTO;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace AuthorVerseServer.Tests.Integrations
{
    public class CommentControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public CommentControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            var currentDirectory = Environment.CurrentDirectory;
            string path = Path.Combine(currentDirectory, "../../../");
            factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseSolutionRelativeContentRoot(path);
            });

            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateComment_ReturnsOkResult()
        {
            // Arrange
            var bookDTO = new CreateCommentDTO
            {
                UserId = "admin",
                BookId = 1,
                Text = "Я и мой комментарий. Почти как Мама, папа я и бд, но только с комментарием",
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Comment/Create", bookDTO);

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
