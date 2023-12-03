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
