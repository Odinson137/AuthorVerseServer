using System.Net;
using System.Net.Http.Headers;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace ServerTests.Integrations;

public class ArtIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ArtIntegrationTests(WebApplicationFactory<Program> factory) 
    {
        var configuration = factory.Services.GetRequiredService<IConfiguration>();

        var token = new CreateJWTtokenService(configuration);
        _client = factory.CreateClient();

        string jwtToken = token.GenerateAdminJwtToken("admin");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
    }
    
    [Fact]
    public async Task GetArts_Ok_ReturnsOkResult()
    {
        var bookId = 1;
        
        // Act
        var response =  await _client.GetAsync($"/api/Art?bookId={bookId}&start=1&end=10");

        var content = await response.Content.ReadAsStringAsync();
        var advertisement = JsonConvert.DeserializeObject<ICollection<ArtDTO>>(content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(advertisement);
    }
    
    [Fact]
    public async Task AddArt_CreateNewArt_ReturnsOkResult()
    {
        var contentFile = new MultipartFormDataContent();

        await using var fileStream = File.OpenRead(@"../../../Images/javascript-it-юмор-geek-5682739.jpeg");
        var fileContent = new StreamContent(fileStream);
        contentFile.Add(fileContent, "Image", "javascript-it-юмор-geek-5682739.jpeg");

        // Добавляем поля модели в форму
        contentFile.Add(new StringContent("1"), "Url");
        contentFile.Add(new StringContent("1"), "Start");
        contentFile.Add(new StringContent("10"), "End");
        contentFile.Add(new StringContent("1"), "BookId");

        var response = await _client.PostAsync("/api/Art", contentFile);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task<int> CreateArt() {
        var contentFile = new MultipartFormDataContent();

        await using var fileStream = File.OpenRead(@"../../../Images/javascript-it-юмор-geek-5682739.jpeg");
        var fileContent = new StreamContent(fileStream);
        contentFile.Add(fileContent, "Image", "javascript-it-юмор-geek-5682739.jpeg");

        // Добавляем поля модели в форму
        contentFile.Add(new StringContent("1"), "Url");
        contentFile.Add(new StringContent("1"), "Start");
        contentFile.Add(new StringContent("10"), "End");
        contentFile.Add(new StringContent("1"), "BookId");

        var response = await _client.PostAsync("/api/Art", contentFile);
        var content = await response.Content.ReadAsStringAsync();

        return int.Parse(content);
    } 
    
    [Fact]
    public async Task Delete_Ok_ReturnsOkResult()
    {
        int id = await CreateArt();
        // Act
        var response =
            await _client.DeleteAsync($"/api/Art?id={id}");
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}