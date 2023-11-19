using AuthorVerseServer.DTO;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Xunit;
using AuthorVerseServer.Data.Enums;

namespace AuthorVerseServer.Tests.Integrations
{
    public class AccoutControllerIntegtationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private CreateJWTtokenService _token;
        private readonly HttpClient _client;

        public AccoutControllerIntegtationTests(WebApplicationFactory<Program> factory)
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
                Assert.True(string.IsNullOrEmpty(selectedBook.Title), "Отсутствует название ");
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
            var uri = "api/Account/GetUserComments";

            var response = await _client.GetAsync(uri);

            var content = await response.Content.ReadAsStringAsync();
            var comments = JsonConvert.DeserializeObject<CommentPageDTO>(content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(comments);

            if (comments.comments.Count > 0)
                Assert.True(comments.PagesCount > 0 );

            foreach (var comment in comments.comments)
            {
                Assert.True(comment.CommentId > 0, "Невозможный id");
                Assert.False(string.IsNullOrEmpty(comment.Text), "Отсутствует текст комментария");
                Assert.True(Enum.IsDefined(typeof(CommentType), comment.CommentType), "Невозможная категория");
                Assert.False(string.IsNullOrEmpty(comment.BookTitle), "Отсутствует название книги");
                Assert.True(comment.ChapterNumber > 0, "Такой главы нет");
                Assert.True(comment.CommentCreatedDateTime != DateOnly.MinValue, "Невозможная дата");
            }
        }

        [Fact]
        public async Task GetUserComments_FullRequest_ReturnsOkResult()
        {
            // Arrange
            var queryParams = new Dictionary<string, string>
            {
                { "commentType", CommentType.Book.ToString() },
                { "page", "2" },
                { "searchComment", "Это мой первый" }
            };

            var queryString = new FormUrlEncodedContent(queryParams);

            var uriBuilder = new UriBuilder("api/Account/UserComments")
            {
                Query = queryString.ReadAsStringAsync().Result
            };

            var uri = uriBuilder.Uri;

            var response = await _client.GetAsync(uri);

            var content = await response.Content.ReadAsStringAsync();
            var comments = JsonConvert.DeserializeObject<CommentPageDTO>(content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(comments);

            Assert.Equal(0, comments.PagesCount);

            foreach (var comment in comments.comments)
            {
                Assert.Contains(comment.Text, queryParams["searchComment"]);
                Assert.Equal(CommentType.Book, comment.CommentType);
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
    }
}
