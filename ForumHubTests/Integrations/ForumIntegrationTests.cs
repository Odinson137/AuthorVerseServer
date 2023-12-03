using AuthorVerseServer.DTO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Net;


namespace ForumHubTests.Integrations;

public class ForumIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ForumIntegrationTests(WebApplicationFactory<Program> factory)
    {

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

        var settings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Error, // Опция по умолчанию
            Error = (sender, args) =>
            {
                args.ErrorContext.Handled = true;
            }
        };

        var messages = JsonConvert.DeserializeObject<ICollection<ForumMessageDTO>>(content, settings);

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
