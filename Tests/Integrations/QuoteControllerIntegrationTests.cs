using AuthorVerseServer.DTO;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Xunit;

namespace AuthorVerseServer.Tests.Integrations
{
    public class QuoteControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CreateJWTtokenService _token;

        public QuoteControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            var currentDirectory = Environment.CurrentDirectory;
            string path = Path.Combine(currentDirectory, "../../../");
            factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseSolutionRelativeContentRoot(path);
            });

            var configuration = factory.Services.GetRequiredService<IConfiguration>();

            _token = new CreateJWTtokenService(configuration);
            _client = factory.CreateClient();

            string jwtToken = _token.GenerateJwtToken("admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        [Fact]
        public async Task GetBookQuotes_GetFullPageOfQuotes_ReturnsOkResult()
        {
            // Arrange
            int bookId = 1;

            // Act
            var response = await _client.GetAsync($"/api/Quote?bookId={bookId}");

            // Assert
            var content = await response.Content.ReadAsStringAsync();
            var quotes = JsonConvert.DeserializeObject<ICollection<QuoteDTO>>(content);

            Assert.NotNull(quotes);
            Assert.True(quotes.Any());
            Assert.True(quotes.Count > 0);

            foreach (var book in quotes)
            {
                Assert.False(string.IsNullOrEmpty(book.Text));
                Assert.False(string.IsNullOrEmpty(book.Quoter.Id));
                Assert.True(book.QuoteCreatedDateTime != DateOnly.MinValue, "Error date");
            }
        }

        [Fact]
        public async Task GetBookQuotes_NotFullContent_ReturnsOkResult()
        {
            // Arrange
            int bookId = 1, page = 100;

            // Act
            var response = await _client.GetAsync($"/api/Quotes?bookId={bookId}&page={page}");

            // Assert
            var content = await response.Content.ReadAsStringAsync();
            var quotes = JsonConvert.DeserializeObject<ICollection<QuoteDTO>>(content);

            Assert.Null(quotes);
        }
    }


}
