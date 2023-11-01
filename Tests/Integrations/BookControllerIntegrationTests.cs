using AuthorVerseServer.Data;
using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Json;
using Xunit;

namespace AuthorVerseServer.Tests.Integrations
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
                AuthorId = "admin",
                GenresId = new List<int> { 1, 2 },
                TagsId = new List<int> { 1, 2 },
                Title = "Берсерк",
                Description = "Черный мечник идёт за тобой",
                AgeRating = AgeRating.EighteenPlus,
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Book/Create", bookDTO);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetMainPopularBooks_ReturnsOkResult()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("/api/Book/MainPopularBooks");

            // Assert
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            ICollection<MainPopularBook>? books = JsonConvert.DeserializeObject<ICollection<MainPopularBook>>(content);
            Assert.True(books is not null, "Books are null");
            
            Assert.True(books.Count > 0, "Count of books is zero");

            foreach (var book in books)
            {
                Assert.True(!string.IsNullOrEmpty(book.Title), "Title not have");
                Assert.True(!string.IsNullOrEmpty(book.Description), "Description not have");
                Assert.True(book.Genres.Count > 0, "Genre's count is zero");
                Assert.True(book.Tags.Count > 0, "Tag's count is zero");
                Assert.True(book.Endings > 0, "Must have not zero endings");
                Assert.True(book.Choices >= 0, "Must have more zero choices");
                Assert.True(book.PublicationData != DateTime.MinValue, "Error date");
            }
        }
    }

}
