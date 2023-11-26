using AuthorVerseServer.DTO;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace AuthorVerseServer.Tests.Integrations
{
    public class QuoteControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public QuoteControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            var currentDirectory = Environment.CurrentDirectory;
            string path = Path.Combine(currentDirectory, "../../../");
            factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseSolutionRelativeContentRoot(path);
            });

            var configuration = factory.Services.GetRequiredService<IConfiguration>();

            var token = new CreateJWTtokenService(configuration);
            _client = factory.CreateClient();

            string jwtToken = token.GenerateJwtToken("admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            _factory = factory;
        }

        [Fact]
        public async Task GetBookQuotes_GetFullPageOfQuotes_ReturnsOkResult()
        {
            // Arrange
            int bookId = 1;

            var redisConnection = _factory.Services.GetRequiredService<IConnectionMultiplexer>();
            var redisDatabase = redisConnection.GetDatabase();
            var value = await redisDatabase.KeyDeleteAsync($"quotes{bookId}-{0}");

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

            var redisConnection = _factory.Services.GetRequiredService<IConnectionMultiplexer>();
            var redisDatabase = redisConnection.GetDatabase();
            await redisDatabase.KeyDeleteAsync($"quotes{bookId}-{page - 1}");

            // Act
            var response = await _client.GetAsync($"/api/Quote?bookId={bookId}&page={page}");

            // Assert
            var content = await response.Content.ReadAsStringAsync();
            var quotes = JsonConvert.DeserializeObject<ICollection<QuoteDTO>>(content);

            Assert.True(quotes.Count == 0);
        }

        [Fact]
        public async Task GetBookQuotes_CachingCheck_ReturnsOkResult()
        {
            // Arrange
            int bookId = 99, page = 99;

            ICollection<QuoteDTO> quotes = new List<QuoteDTO>()
            {
                new QuoteDTO()
            }; 

            var redisConnection = _factory.Services.GetRequiredService<IConnectionMultiplexer>();
            var redisDatabase = redisConnection.GetDatabase();
            await redisDatabase.StringSetAsync($"quotes{bookId}-{page - 1}", JsonConvert.SerializeObject(quotes));

            // Act
            var cacheResponse = await _client.GetAsync($"/api/Quote?bookId={bookId}&page={page}");

            // Assert
            var content = await cacheResponse.Content.ReadAsStringAsync();
            var checkQuotes = JsonConvert.DeserializeObject<ICollection<QuoteDTO>>(content);

            Assert.NotNull(checkQuotes);
            Assert.True(checkQuotes.Count == quotes.Count);
        }

        [Fact]
        public async Task PostNewBookQuote_CreateQuote_ReturnsOkResult()
        {
            // Arrange
            int bookId = 1;
            string text = "Эта цитата должна потом удалить";
            StringContent requestContent = new StringContent("", Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"/api/Quote?bookId={bookId}&text={text}", requestContent);

            // Assert
            var content = await response.Content.ReadAsStringAsync();
            Assert.True(int.TryParse(content, out int i));
            Assert.True(i > 0);
        }

        [Fact]
        public async Task DeleteBookQuote_CreateAndDeleteThisQuote_ReturnsOkResult()
        {
            // Arrange
            int bookId = 1;
            string text = "Эта цитата должна потом удалить";
            StringContent requestContent = new StringContent("", Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"/api/Quote?bookId={bookId}&text={text}", requestContent);

            // Assert
            var content = await response.Content.ReadAsStringAsync();
            int quoteId = int.Parse(content);
            var deleteResponse = await _client.DeleteAsync($"/api/Quote?quoteId={quoteId}");
            Assert.True(deleteResponse.IsSuccessStatusCode);

        }
    }


}
