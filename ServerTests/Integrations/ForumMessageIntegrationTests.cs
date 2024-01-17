using AuthorVerseServer.DTO;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Net;


namespace ServerApiForumTests.Integrations;

public class ForumMessageIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public ForumMessageIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
    }

    [Fact]
    public async Task GetMessages_CheckOkRequest_ReturnsOkResult()
    {
        // Arrange
        var response = await _client.GetAsync("/api/ForumMessage?bookId=1&page=1");

        var content = await response.Content.ReadAsStringAsync();

        var messages = JsonConvert.DeserializeObject<ICollection<ForumMessageDTO>>(content);

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
    public async Task GetToParentMessages_CheckOkRequest_ReturnsOkResult()
    {
        // Arrange
        var response = await _client.GetAsync("/api/ForumMessage/GetToParent?bookId=1&lastMessageId=1&parentMessageId=87");

        var content = await response.Content.ReadAsStringAsync();

        var messages = JsonConvert.DeserializeObject<ICollection<ForumMessageDTO>>(content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(messages);

        foreach (var message in messages)
        {
            Assert.False(string.IsNullOrEmpty(message.ViewName), "Отсутствует автор");
            Assert.False(string.IsNullOrEmpty(message.Text), "Отсутствует название");
            Assert.True(message.SendTime != DateTime.MinValue, "Невозможная дата");
        }
    }
}
