using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ServerTests.Integrations
{
    public class CommentRatingControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public CommentRatingControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();

            var configuration = factory.Services.GetRequiredService<IConfiguration>();

            var token = new CreateJWTtokenService(configuration);

            string jwtToken = token.GenerateJwtToken("admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        [Fact]
        public async Task DeleteRating_Ok_ReturnsOkResult()
        {
            // Arrange
            int commentId = 1;

            await _client.PostAsJsonAsync($"/api/CommentRating/Up?commentId={commentId}", (HttpContent?)null);

            // Act
            var response = await _client.DeleteAsync($"/api/CommentRating/Delete?commentId={commentId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task Delete_ChangeFromUpToRun_ReturnsOkResult()
        {
            // Arrange
            int commentId = -1;

            // Act
            var response = await _client.DeleteAsync($"/api/CommentRating/Delete?commentId={commentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task ChangeUpRating_Ok_ReturnsOkResult()
        {
            // Arrange
            int commentId = 1;
            await _client.DeleteAsync($"/api/CommentRating/Delete?commentId={commentId}");

            // Act
            var response = await _client.PostAsJsonAsync($"/api/CommentRating/Up?commentId={commentId}", (HttpContent?) null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ChangeUpRating_AlreadyExist_ReturnsBadRequest()
        {
            // Arrange
            int commentId = 1;
            await _client.PostAsJsonAsync($"/api/CommentRating/Up?commentId={commentId}", (HttpContent?)null);

            // Act
            var response = await _client.PostAsJsonAsync($"/api/CommentRating/Up?commentId={commentId}", (HttpContent?)null);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("Already exist", content);
        }


        [Fact]
        public async Task ChangeUpRating_ChangeRating_ReturnsBadRequest()
        {
            // Arrange
            int commentId = 1;
            await _client.PostAsJsonAsync($"/api/CommentRating/Down?commentId={commentId}", (HttpContent?)null);

            // Act
            var response = await _client.PostAsJsonAsync($"/api/CommentRating/Up?commentId={commentId}", (HttpContent?)null);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ChangeDownRating_Ok_ReturnsOkResult()
        {
            // Arrange
            int commentId = 1;
            await _client.DeleteAsync($"/api/CommentRating/Delete?commentId={commentId}");

            // Act
            var response = await _client.PostAsJsonAsync($"/api/CommentRating/Down?commentId={commentId}", (HttpContent?)null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ChangeDownRating_AlreadyExist_ReturnsOkResult()
        {
            // Arrange
            int commentId = 1;
            await _client.PostAsJsonAsync($"/api/CommentRating/Down?commentId={commentId}", (HttpContent?)null);

            // Act
            var response = await _client.PostAsJsonAsync($"/api/CommentRating/Down?commentId={commentId}", (HttpContent?)null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ChangeDownRating_ChangeFromUpToDown_ReturnsOkResult()
        {
            // Arrange
            int commentId = 1;
            await _client.PostAsJsonAsync($"/api/CommentRating/Up?commentId={commentId}", (HttpContent?)null);

            // Act
            var response = await _client.PostAsJsonAsync($"/api/CommentRating/Down?commentId={commentId}", (HttpContent?)null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


    }
}
