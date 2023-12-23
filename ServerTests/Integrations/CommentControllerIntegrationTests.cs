using AuthorVerseServer.DTO;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using AuthorVerseServer.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Azure.Core;
using System.Net.Http.Json;
using AuthorVerseServer.Data.Enums;

namespace ServerTests.Integrations;

public class CommentControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CreateJWTtokenService _token;
    public CommentControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();

        var configuration = factory.Services.GetRequiredService<IConfiguration>();

        _token = new CreateJWTtokenService(configuration);
    }


    [Fact]
    public async Task GetBookComments_NotAuthorize_ReturnsOkResult()
    {
        // Arrange
        //string jwtToken = _token.GenerateJwtToken("admin");
        //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        //var requestContent = new StringContent(JsonConvert.SerializeObject(bookDTO), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.GetAsync("/api/Comment?bookId=1&page=1");
        var content = await response.Content.ReadAsStringAsync();
        var comments = JsonConvert.DeserializeObject<ICollection<CommentDTO>>(content);
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(comments);
        Assert.True(comments.First().IsRated == 0);
    }

    [Fact]
    public async Task GetBookComments_Authorize_ReturnsOkResult()
    {
        // Arrange
        string jwtToken = _token.GenerateJwtToken("admin");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        // Act
        var response = await _client.GetAsync("/api/Comment?bookId=1&page=1");
        var content = await response.Content.ReadAsStringAsync();
        var comments = JsonConvert.DeserializeObject<ICollection<CommentDTO>>(content);
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(comments);
        Assert.True(comments.First().IsRated != 0);
    }

    [Fact]
    public async Task CreateComment_CheckModelStateWork_ReturnsBadResult()
    {
        // Arrange
        var bookDTO = new CreateCommentDTO
        {
            BookId = 0,
            Text = "Слишком коротко",
        };

        string jwtToken = _token.GenerateJwtToken("admin");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var requestContent = new StringContent(JsonConvert.SerializeObject(bookDTO), Encoding.UTF8, "application/json");
        
        // Act
        var response = await _client.PostAsync("/api/Comment/Create", requestContent);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public async Task<int> CreateBook()
    {
        var bookDTO = new CreateBookDTO
        {
            GenresId = new List<int> { 1, 2 },
            TagsId = new List<int> { 1, 2 },
            Title = "Берсерк",
            Description = "«Берсерк» - это японская манга, созданная Кентаро Миурой. Сюжет рассказывает о Гатсе, мстительном воине, путешествующем в мрачном мире средневековой Европы, сражаясь с демонами и чудовищами. Волнующий и темный рассказ о выживании, предательстве и потере человечности, \"Берсерк\" славится своим уникальным стилем и глубокими темами, привлекая миллионы читателей по всему миру.",
            AgeRating = AgeRating.EighteenPlus,
        };

        var response = await _client.PostAsJsonAsync("/api/Book/Create", bookDTO);
        var content = await response.Content.ReadAsStringAsync();
        return int.Parse(content);
    }

    [Fact]
    public async Task CreateComment_ReturnsOkResult()
    {
        // Arrange
        string jwtToken = _token.GenerateJwtToken("admin");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        int bookId = await CreateBook();

        var bookDTO = new CreateCommentDTO
        {
            BookId = bookId,
            Text = "Я и мой комментарий. Почти как Мама, папа я и бд, но только с комментарием, длинна которого должна быть ",
            Rating = 4,
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Comment/Create", bookDTO);
        var context = await response.Content.ReadAsStringAsync();
        // Assert
        response.EnsureSuccessStatusCode();
    }
}
