using AuthorVerseServer.DTO;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using AuthorVerseServer.Data.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Server.Tests.Integrations;
public class AccoutControllerIntegtationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private CreateJWTtokenService _token;
    private readonly HttpClient _client;

    public AccoutControllerIntegtationTests(WebApplicationFactory<Program> factory)
    {
        var configuration = factory.Services.GetRequiredService<IConfiguration>();

        _token = new CreateJWTtokenService(configuration);
        _client = factory.CreateClient();

        string jwtToken = _token.GenerateJwtToken("admin");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
    }

    [Fact]
    public async Task GetUserProfile_Ok_ReturnsOkResult()
    {
        // Act
        var response = await _client.GetAsync("/api/Account/Profile");

        var content = await response.Content.ReadAsStringAsync();
        var userProfile = JsonConvert.DeserializeObject<UserProfileDTO>(content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(userProfile);
        Assert.False(string.IsNullOrEmpty(userProfile.UserName));
    }

    [Fact]
    public async Task SelectedBooks_CheckOkRequest_ReturnsOkResult()
    {
        // Arrange
        var response = await _client.GetAsync("/api/Account/SelectedBooks");

        var content = await response.Content.ReadAsStringAsync();
        var selectedBooks = JsonConvert.DeserializeObject<ICollection<SelectedUserBookDTO>>(content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(selectedBooks);

        foreach (var selectedBook in selectedBooks)
        {
            Assert.True(selectedBook.BookId > 0, "Невозможный id");
            Assert.False(string.IsNullOrEmpty(selectedBook.Title), "Отсутствует название ");
            Assert.True(Enum.IsDefined(typeof(BookState), selectedBook.BookState), "Невозможная категория книги");
            Assert.True(selectedBook.PublicationData != DateOnly.MinValue, "Невозможная дата");
            Assert.True(selectedBook.LastReadingChapter > 0, "Такой главы нет");
            Assert.True(selectedBook.LastBookChapter > 0, "Такой главы нет");
        }
    }

    [Fact]
    public async Task GetUserComments_EmptyRequest_ReturnsOkResult()
    {
        // Arrange
        var uri = "api/Account/UserComments";

        var response = await _client.GetAsync(uri);

        var content = await response.Content.ReadAsStringAsync();
        var comments = JsonConvert.DeserializeObject<CommentPageDTO>(content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(comments);

        if (comments.Comments.Count > 0)
            Assert.True(comments.PagesCount > 0 );


        foreach (var comment in comments.Comments)
        {
            Assert.True(comment.BaseId > 0, "Невозможный id");
            Assert.False(string.IsNullOrEmpty(comment.Text), "Отсутствует текст комментария");
            Assert.True(Enum.IsDefined(typeof(CommentType), comment.CommentType), "Невозможная категория");
            Assert.False(string.IsNullOrEmpty(comment.BookTitle), "Отсутствует название книги");
            Assert.True(comment.CreatedDateTime != DateOnly.MinValue, "Невозможная дата");
        }
    }

    [Fact]
    public async Task GetUserComments_FullRequest_ReturnsOkResult()
    {
        // Arrange
        string uri = "api/Account/UserComments?commentType=1&page=2&searchComment=";

        var response = await _client.GetAsync(uri);

        var content = await response.Content.ReadAsStringAsync();
        var comments = JsonConvert.DeserializeObject<CommentPageDTO>(content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(comments);

        Assert.Equal(0, comments.PagesCount);

        foreach (var comment in comments.Comments)
        {
            //Assert.Contains(comment.Text, queryParams["searchComment"]);
            Assert.Equal(CommentType.Chapter, comment.CommentType);
        }
    }


    [Fact]
    public async Task GetUserComments_SearchTextFromCommentText_ReturnsOkResult()
    {
        // Arrange
        string uri = "api/Account/UserComments?commentType=0&page=2&searchComment=Это мой первый";

        var response = await _client.GetAsync(uri);

        var content = await response.Content.ReadAsStringAsync();
        var comments = JsonConvert.DeserializeObject<CommentPageDTO>(content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(comments);

        Assert.Equal(0, comments.PagesCount);

        foreach (var comment in comments.Comments)
        {
            Assert.Contains("Это мой первый", comment.Text, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(CommentType.Book, comment.CommentType);
        }
    }

    [Fact]
    public async Task GetUserComments_SearchTextFromNoteText_ReturnsOkResult()
    {
        // Arrange
        string uri = "api/Account/UserComments?commentType=0&page=2&searchComment=Когда же выйдет";

        var response = await _client.GetAsync(uri);

        var content = await response.Content.ReadAsStringAsync();
        var comments = JsonConvert.DeserializeObject<CommentPageDTO>(content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(comments);

        Assert.Equal(0, comments.PagesCount);

        foreach (var comment in comments.Comments)
        {
            Assert.Contains("Когда же выйдет", comment.Text, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(CommentType.Chapter, comment.CommentType);
        }
    }

    [Fact]
    public async Task GetFriends_Ok_ReturnsOkResult()
    {
        // Arrange
        var uri = "api/Account/Friends";

        var response = await _client.GetAsync(uri);

        var content = await response.Content.ReadAsStringAsync();
        var friends = JsonConvert.DeserializeObject<ICollection<FriendDTO>>(content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(friends);

        foreach (var friend in friends)
        {
            Assert.False(string.IsNullOrEmpty(friend.Id), "id друга отсутсвует");
            Assert.False(string.IsNullOrEmpty(friend.UserName), "должен быть User Name");
            Assert.True(friend.FriendShipTime != DateOnly.MinValue, "Невозможная дата");
        }
    }

    [Fact]
    public async Task GetUserBooks_Ok_ReturnsOkResult()
    {
        // Arrange
        var response = await _client.GetAsync("/api/Account/UserBooks");

        var content = await response.Content.ReadAsStringAsync();
        var books = JsonConvert.DeserializeObject<ICollection<UserBookDTO>>(content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(books);

        foreach (var book in books)
        {
            Assert.False(string.IsNullOrEmpty(book.Title));
            Assert.False(string.IsNullOrEmpty(book.Description));
            Assert.True(book.BookId > 0);
            Assert.True(book.PublicationData != DateOnly.MinValue, "Невозможная дата");
        }
    }

    [Fact]
    public async Task GetUserBooksUpdates_Ok_ReturnsOkResult()
    {
        // Arrange
        var uri = "api/Account/Updates";

        var response = await _client.GetAsync(uri);

        var content = await response.Content.ReadAsStringAsync();
        var books = JsonConvert.DeserializeObject<ICollection<UpdateAccountBook>>(content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(books);

        Assert.True(books.Count > 0 && books.Count != 10);

        foreach (var book in books)
        {
            Assert.False(string.IsNullOrEmpty(book.BookTitle));
            Assert.True(book.BookId > 0);
            Assert.False(string.IsNullOrEmpty(book.BookTitle));
            Assert.True(book.ChapterNumber > 0);
        }
    }

    [Fact]
    public async Task ChangeUserProfile_PasswordDoesNotMatch_ReturnsOkResult()
    {
        // Arrange
        var userProfile = new EditProfileDTO()
        {
            Name = "Yuri",
            LastName = "Metanit",
            CheckPassword = "ErrorPaswword@123",
            Description = "Люблю жизнь, она моя, она нагнула меня, но я не отчаиваюсь, живу",
            Password = "Password@123",
        };

        var uri = "api/Account/ProfileChange";

        var response = await _client.PutAsJsonAsync(uri, userProfile);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ChangeUserProfile_ChangeUser_ReturnsOkResult()
    {
        // Arrange
        var userProfile = new EditProfileDTO()
        {
            Name = "Yuri",
            LastName = "Metanit",
            Password = "Password@123",
            CheckPassword = "Password@123",
            Description = "Люблю жизнь, она моя, она нагнула меня, но я не отчаиваюсь, живу",
        };

        var response = await _client.PutAsJsonAsync("api/Account/ProfileChange", userProfile);
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
