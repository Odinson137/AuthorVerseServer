using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Services;
using Azure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using static Google.Apis.Requests.BatchRequest;

namespace ServerTests.Integrations;

public class BooksControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CreateJWTtokenService _token;

    public BooksControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();

        var configuration = factory.Services.GetRequiredService<IConfiguration>();

        _token = new CreateJWTtokenService(configuration);

        string jwtToken = _token.GenerateJwtToken("admin");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
    }

    [Fact]
    public async Task CreateBook_CheckModelStateWork_ReturnsBadRequest()
    {
        // Arrange
        var bookDTO = new CreateBookDTO
        {
            GenresId = new List<int> { 1, 2 },
            TagsId = new List<int> { 1, 2 },
            Title = string.Empty,
            Description = "«Берсерк» - это японская манга, созданная Кентаро Миурой. Сюжет рассказывает о Гатсе, мстительном воине, путешествующем в мрачном мире средневековой Европы, сражаясь с демонами и чудовищами. Волнующий и темный рассказ о выживании, предательстве и потере человечности, \"Берсерк\" славится своим уникальным стилем и глубокими темами, привлекая миллионы читателей по всему миру.",
            AgeRating = AgeRating.EighteenPlus,
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Book/Create", bookDTO);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateBook_ReturnsOkResult()
    {
        // Arrange
        var bookDTO = new CreateBookDTO
        {
            GenresId = new List<int> { 1, 2 },
            TagsId = new List<int> { 1, 2 },
            Title = "Берсерк",
            Description = "«Берсерк» - это японская манга, созданная Кентаро Миурой. Сюжет рассказывает о Гатсе, мстительном воине, путешествующем в мрачном мире средневековой Европы, сражаясь с демонами и чудовищами. Волнующий и темный рассказ о выживании, предательстве и потере человечности, \"Берсерк\" славится своим уникальным стилем и глубокими темами, привлекая миллионы читателей по всему миру.",
            AgeRating = AgeRating.EighteenPlus,
        };

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
        int page = 1;

        // Act
        var response = await _client.GetAsync($"/api/Book/SearchBy?tagId={tagId}&genreId={genreid}&page={page}");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Theory]
    [InlineData(0, 0, 1)]
    [InlineData(1, 1, 2)]
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
        int genreId = 0, page = 1;

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
        int page = 1;

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
    public async Task GetPageBooksByGenreAndTags_GetTrueInfromation_ReturnsOkResult()
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
        }
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
            Assert.Contains(tagId, book.Tags.Select(tag => tag.TagId));
            Assert.Contains(genreId, book.Genres.Select(genre => genre.GenreId));
        }
    }

    [Fact]
    public async Task GetPageBooksByGenreAndTagsAndTitle_Ok_ReturnsOkResult()
    {
        // Arrange
        int tagId = 0;
        int genreId = 0;
        int page = 1;
        string searchText = "19";

        // Act
        var response = await _client.GetAsync($"/api/Book/SearchBy?tagId={tagId}&genreId={genreId}&page={page}&searchText={searchText}");

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        var booksPage = JsonConvert.DeserializeObject<BookPageDTO>(content);

        Assert.NotNull(booksPage);
        Assert.NotEmpty(booksPage.Books);

        foreach (var book in booksPage.Books)
        {
            Assert.Contains(searchText, book.Title);
        }
    }

    [Fact]
    public async Task GetAuthorBooks_Ok_ReturnsOkResult()
    {
        // Arrange
        string userId = "admin";

        // Act
        var response = await _client.GetAsync($"/api/Book/AuthorBooks?authorId={userId}");

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        var books = JsonConvert.DeserializeObject<ICollection<MinimalBook>>(content);

        Assert.NotNull(books);
        Assert.True(books.Any());
        Assert.True(books.Count() > 0);

        foreach (var book in books)
        {
            Assert.NotEmpty(book.Title);
            Assert.True(book.BookId > 0);
        }
    }

    [Fact]
    public async Task GetSimilarBooks_GoodData_ReturnsOkResult()
    {
        // Arrange
        int bookId = 1;
        // Act
        var response = await _client.GetAsync($"/api/Book/SimilarBooks?bookId={bookId}");

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        var books = JsonConvert.DeserializeObject<ICollection<MinimalBook>>(content);

        Assert.NotNull(books);
        Assert.True(books.Any());
        Assert.True(books.Count() > 0);

        foreach (var b in books)
        {
            Assert.NotEmpty(b.Title);
            Assert.True(b.BookId > 0);
        }
    }

    [Fact]
    public async Task GetShortBook_Ok_ReturnResult()
    {
        // Arrange
        int bookId = 1;

        // Act
        var responseBook = await _client.GetAsync($"/api/Book/Short/{bookId}");
        var contentBook = await responseBook.Content.ReadAsStringAsync();
        var book = JsonConvert.DeserializeObject<ShoptBookDTO>(contentBook);

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseBook.StatusCode);
        Assert.NotNull(book);
        Assert.False(string.IsNullOrEmpty(book.Title));
        Assert.False(string.IsNullOrEmpty(book.AuthorName));
    }

    [Fact]
    public async Task GetShortBook_NotFound_ReturnNotFound()
    {
        // Arrange
        int bookId = -1;

        // Act
        var responseBook = await _client.GetAsync($"/api/Book/Short/{bookId}");
        var contentBook = await responseBook.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, responseBook.StatusCode);
    }

    [Fact]
    public async Task GetDetailBook_Ok_ReturnResult()
    {
        // Arrange
        int bookId = 1;

        // Act
        var responseBook = await _client.GetAsync($"/api/Book/Detail/{bookId}");
        var contentBook = await responseBook.Content.ReadAsStringAsync();
        var book = JsonConvert.DeserializeObject<DetailBookDTO>(contentBook);

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseBook.StatusCode);
        Assert.NotNull(book);
        Assert.NotNull(book.Author);
        Assert.False(string.IsNullOrEmpty(book.Title));
    }

    [Fact]
    public async Task GetDetailBook_NotFound_ReturnNotFound()
    {
        // Arrange
        int bookId = -1;

        // Act
        var responseBook = await _client.GetAsync($"/api/Book/Short/{bookId}");
        var contentBook = await responseBook.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, responseBook.StatusCode);
    }

    public async Task<DetailBookDTO> GetBook(int bookId)
    {
        var responseBook = await _client.GetAsync($"/api/Book/Detail/{bookId}");
        var contentBook = await responseBook.Content.ReadAsStringAsync();
        var book = JsonConvert.DeserializeObject<DetailBookDTO>(contentBook);
        return book;
    }

    [Fact]
    public async Task GetSimilarBooks_OkSimilar_ReturnsOkResult()
    {
        // Arrange
        int bookId = 1;
        var book = await GetBook(bookId);
        var bookGenres = book.Genres.Select(x => x.GenreId);
        var bookTags = book.Tags.Select(x => x.TagId);

        // Act
        var response = await _client.GetAsync($"/api/Book/SimilarBooks?bookId={bookId}");

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        var books = JsonConvert.DeserializeObject<ICollection<MinimalBook>>(content);

        foreach (var similarBook in books)
        {
            var checkBook = await GetBook(similarBook.BookId);
            var checkGenresBook = checkBook.Genres.Select(x => x.GenreId);
            bool hasCommonGenre = checkGenresBook.Any(similarGenre => bookGenres.Contains(similarGenre));
            Assert.True(hasCommonGenre, $"No common genre found between the original book and similar book {similarBook.BookId}");

            var checkTagsBook = checkBook.Tags.Select(x => x.TagId);
            bool hasCommonTag = checkTagsBook.Any(similarTag => bookTags.Contains(similarTag));
            Assert.True(hasCommonTag, $"No common tag found between the original book and similar book {similarBook.BookId}");
        }
    }
}
