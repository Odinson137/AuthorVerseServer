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

namespace Server.Tests.Integrations;

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
        var response = await _client.PostAsync("/api/Comment?bookId=1&page=1", new StringContent(""));
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
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

    [Fact]
    public async Task CreateComment_ReturnsOkResult()
    {
        // Arrange
        Random random = new Random();

        var bookDTO = new CreateCommentDTO
        {
            BookId = random.Next(1, 1000),
            Text = "Я и мой комментарий. Почти как Мама, папа я и бд, но только с комментарием, длинна которого должна быть ",
        };

        // Act
        string jwtToken = _token.GenerateJwtToken("admin");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var requestContent = new StringContent(JsonConvert.SerializeObject(bookDTO), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/Comment/Create", requestContent);

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
