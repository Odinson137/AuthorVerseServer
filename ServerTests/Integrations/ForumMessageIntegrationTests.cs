using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Services;
using Castle.Core.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;


namespace ServerTests.Integrations;

public class ForumMessageIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ForumMessageIntegrationTests(WebApplicationFactory<Program> factory)
    {
        //var configuration = factory.Services.GetRequiredService<IConfiguration>();

        //_token = new CreateJWTtokenService(configuration);
        _client = factory.CreateClient();

        //string jwtToken = _token.GenerateJwtToken("admin");
        //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
    }

    [Fact]
    public async Task GetMessages_CheckOkRequest_ReturnsOkResult()
    {
        // Arrange
        var response = await _client.GetAsync("/api/ForumMessage?bookId=1&page=1");

        var content = await response.Content.ReadAsStringAsync();
        string a = "asda";
        var messages = JsonConvert.DeserializeObject<ICollection<ForumMessageDTO>>(a);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(messages);

        foreach (var message in messages)
        {
            Assert.False(string.IsNullOrEmpty(message.ViewName), "Отсутствует автор");
            Assert.False(string.IsNullOrEmpty(message.Text), "Отсутствует название");
            Assert.True(message.SendTime != DateTime.MinValue, "Невозможная дата");
        }
    }

    [Fact]
    public async Task AddMessage_CheckOkRequest_ReturnsOkResult()
    {
        // Arrange
        string key = "3aedasdsad";
        var response = await _client.PostAsync($"/api/ForumMessage?key={key}", null);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(int.TryParse(content, out int value));
    }

    [Fact]
    public async Task PutMessage_CheckOkRequest_ReturnsOkResult()
    {
        // Arrange
        string key = "3aedasdsad";
        var response = await _client.PutAsync($"/api/ForumMessage?key={key}", null);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(int.TryParse(content, out int value));
    }

}
