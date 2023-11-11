﻿using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace AuthorVerseServer.Tests.Integrations
{
    public class BooksControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CreateJWTtokenService _token;

        public BooksControllerIntegrationTests(WebApplicationFactory<Program> factory)
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
        public async Task CreateBook_CheckModelStateWork_ReturnsBadRequest()
        {
            // Arrange
            var bookDTO = new CreateBookDTO
            {
                AuthorId = string.Empty,
                GenresId = new List<int> { 1, 2 },
                TagsId = new List<int> { 1, 2 },
                Title = "Берсерк",
                Description = "«Берсерк» - это японская манга, созданная Кентаро Миурой. Сюжет рассказывает о Гатсе, мстительном воине, путешествующем в мрачном мире средневековой Европы, сражаясь с демонами и чудовищами. Волнующий и темный рассказ о выживании, предательстве и потере человечности, \"Берсерк\" славится своим уникальным стилем и глубокими темами, привлекая миллионы читателей по всему миру.",
                AgeRating = AgeRating.EighteenPlus,
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Book/Create", bookDTO);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
                Description = "«Берсерк» - это японская манга, созданная Кентаро Миурой. Сюжет рассказывает о Гатсе, мстительном воине, путешествующем в мрачном мире средневековой Европы, сражаясь с демонами и чудовищами. Волнующий и темный рассказ о выживании, предательстве и потере человечности, \"Берсерк\" славится своим уникальным стилем и глубокими темами, привлекая миллионы читателей по всему миру.",
                AgeRating = AgeRating.EighteenPlus,
            };

            string jwtToken = _token.GenerateJwtToken("admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var requestContent = new StringContent(JsonConvert.SerializeObject(bookDTO), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/Book/Create", requestContent);

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

        [Fact]
        public async Task GetPageBooksByGenreAndTags_SendRequest_ReturnsOkResult()
        {
            // Arrange
            int tagId = 0;
            int genreid = 0;
            int page = 0;

            // Act
            var response = await _client.GetAsync($"/api/Book/SearchBy?tagId={tagId}&genreId={genreid}&page={page}");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(1, 1, 1)]
        public async Task GetPageBooksByGenreAndTags_GetObject_ReturnsOkResult(int tagId, int genreId, int page)
        {
            // Arrange

            // Act
            var response = await _client.GetAsync($"/api/Book/SearchBy?tagId={tagId}&genreId={genreId}&page={page}");

            // Assert
            var content = await response.Content.ReadAsStringAsync();
            var booksPage = JsonConvert.DeserializeObject<BookPageDTO>(content);

            Assert.NotNull(booksPage);
            Assert.NotEmpty(booksPage.Books);
        }

        [Fact]
        public async Task GetPageBooksByGenreAndTags_BadTagId_ReturnsZeroBooksCount()
        {
            // Arrange
            int tagId = 60;
            int genreId = 0, page = 0;

            // Act
            var response = await _client.GetAsync($"/api/Book/SearchBy?tagId={tagId}&genreId={genreId}&page={page}");

            // Assert
            var content = await response.Content.ReadAsStringAsync();
            var booksPage = JsonConvert.DeserializeObject<BookPageDTO>(content);

            Assert.NotNull(booksPage);
            Assert.Empty(booksPage.Books);
        }

        [Fact]
        public async Task GetPageBooksByGenreAndTags_GetCount_ReturnsOkResult()
        {
            // Arrange
            int tagId = 0;
            int genreId = 0;
            int page = 0;

            // Act
            var response = await _client.GetAsync($"/api/Book/SearchBy?tagId={tagId}&genreId={genreId}&page={page}");

            // Assert
            var content = await response.Content.ReadAsStringAsync();
            var booksPage = JsonConvert.DeserializeObject<BookPageDTO>(content);

            Assert.NotNull(booksPage);
            Assert.NotEmpty(booksPage.Books);
            Assert.True(booksPage.BooksCount > 0, "BooksCount is zero");
        }

        [Fact]
        public async Task GetPageBooksByGenreAndTags_GetNullGenreTag_ReturnsOkResult()
        {
            // Arrange
            int tagId = 0;
            int genreId = 0;
            int page = 1;

            // Act
            var response = await _client.GetAsync($"/api/Book/SearchBy?tagId={tagId}&genreId={genreId}&page={page}");

            // Assert
            var content = await response.Content.ReadAsStringAsync();
            var booksPage = JsonConvert.DeserializeObject<BookPageDTO>(content);

            Assert.NotNull(booksPage);
            Assert.NotEmpty(booksPage.Books);

            foreach (var book in booksPage.Books)
            {
                Assert.True(!string.IsNullOrEmpty(book.Title), "Title not have");
                Assert.True(book.Genres.Count > 0, "Genre's count is zero");
                Assert.True(book.Tags.Count > 0, "Tag's count is zero");
                Assert.DoesNotContain(tagId, book.Tags.Select(tag => tag.TagId));
                Assert.DoesNotContain(genreId, book.Genres.Select(genre => genre.GenreId));
            }
        }

        [Fact]
        public async Task GetPageBooksByGenreAndTags_Ok_ReturnsOkResult()
        {
            // Arrange
            int tagId = 1;
            int genreId = 4;
            int page = 1;

            // Act
            var response = await _client.GetAsync($"/api/Book/SearchBy?tagId={tagId}&genreId={genreId}&page={page}");

            // Assert
            var content = await response.Content.ReadAsStringAsync();
            var booksPage = JsonConvert.DeserializeObject<BookPageDTO>(content);

            Assert.NotNull(booksPage);
            Assert.NotEmpty(booksPage.Books);

            foreach (var book in booksPage.Books)
            {
                Assert.True(!string.IsNullOrEmpty(book.Title), "Title not have");
                Assert.True(book.Genres.Count > 0, "Genre's count is zero");
                Assert.True(book.Tags.Count > 0, "Tag's count is zero");
                Assert.Contains(tagId, book.Tags.Select(tag => tag.TagId));
                Assert.Contains(genreId, book.Genres.Select(genre => genre.GenreId));
            }
        }
    }
}