using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Services;
using Castle.Core.Configuration;
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
    public async Task AddMessage_AddToNullParrent_ReturnsOkResult()
    {
        // Arrange
        string newGuid = Guid.NewGuid().ToString();
        string key = $"add_message:{newGuid}";

        SendForumMessageDTO sendMessage = new SendForumMessageDTO
        {
            BookId = 1,
            Text = "Hello",
            UserId = "admin",
            AnswerId = null,
        };

        var redisConnection = _factory.Services.GetRequiredService<IConnectionMultiplexer>();
        var redis = redisConnection.GetDatabase();
        await redis.StringSetAsync(key, JsonConvert.SerializeObject(sendMessage), TimeSpan.FromSeconds(10));

        var response = await _client.PostAsync($"/api/ForumMessage?key={newGuid}", null);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(int.TryParse(content, out int value));
        Assert.True(value > 0);
    }

    [Fact]
    public async Task AddMessage_AddWithParrent_ReturnsOkResult()
    {
        // Arrange
        Guid newGuid = Guid.NewGuid();
        string key = $"add_message:{newGuid}";

        SendForumMessageDTO sendMessage = new SendForumMessageDTO
        {
            BookId = 1,
            Text = "Hello",
            UserId = "admin",
            AnswerId = 1,
        };

        var redisConnection = _factory.Services.GetRequiredService<IConnectionMultiplexer>();
        var redis = redisConnection.GetDatabase();
        await redis.StringSetAsync(key, JsonConvert.SerializeObject(sendMessage), TimeSpan.FromSeconds(10));

        var response = await _client.PostAsync($"/api/ForumMessage?key={newGuid}", null);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(int.TryParse(content, out int value));
        Assert.True(value > 0);
    }

    [Fact]
    public async Task PutMessage_CheckOkRequest_ReturnsOkResult()
    {
        // Arrange
        Guid newGuid = Guid.NewGuid();
        string key = $"put_message:{newGuid}";

        var changeTextMessage = new ChangeTextDTO
        {
            MessageId = 1,
            NewText = "Hello from new message text",
        };

        var redisConnection = _factory.Services.GetRequiredService<IConnectionMultiplexer>();
        var redis = redisConnection.GetDatabase();
        await redis.StringSetAsync(key, JsonConvert.SerializeObject(changeTextMessage), TimeSpan.FromSeconds(10));

        var response = await _client.PutAsync($"/api/ForumMessage?key={newGuid}", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

}
