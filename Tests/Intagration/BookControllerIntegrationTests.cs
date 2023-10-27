using AuthorVerseServer.DTO;
using AuthorVerseServer.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace AuthorVerseServer.Tests.Intagration
{
    public class BooksControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public BooksControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.UseSolutionRelativeContentRoot("C:\\Users\\buryy\\source\\repos\\AuthorVerseServer");
            }).CreateClient();
        }

        [Fact]
        public async Task GetCountBooks_ReturnsCorrectCount()
        {
            // Act
            var response = await _client.GetAsync("/api/Book/Count");

            // Assert
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            Assert.True(int.TryParse(content, out int bookCount));

            Assert.True(bookCount >= 0);
        }

        [Fact]
        public async Task CreateBook_ReturnsOkResult()
        {
            // Arrange
            var bookDTO = new CreateBookDTO
            {
                AuthorId = "3f1dea02-0436-4570-8718-51596e4b2987",
                GenresId = new List<int> { 17, 18 },
                Title = "Берсерк",
                Description = "Черный мечник идёт за тобой",
                AgeRating = AgeRating.EighteenPlus,
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Book/Create", bookDTO);
            
            // Assert
            response.EnsureSuccessStatusCode();
        }
    }

}
