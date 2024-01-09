
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

public class AdvertisementsIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AdvertisementsIntegrationTests(WebApplicationFactory<Program> factory)
    {
        var configuration = factory.Services.GetRequiredService<IConfiguration>();

        var token = new CreateJWTtokenService(configuration);
        _client = factory.CreateClient();

        string jwtToken = token.GenerateAdminJwtToken("admin");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
    }
    
    [Fact]
    public async Task GetBookAdvertisement_Ok_ReturnsOkResult()
    {
        var bookId = 1;
        
        // Act
        var response =
            await _client.PatchAsync($"/api/Advertisement?bookId={bookId}", null);

        var content = await response.Content.ReadAsStringAsync();
        var advertisement = JsonConvert.DeserializeObject<AdvertisementDTO>(content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(advertisement);
        Assert.False(string.IsNullOrEmpty(advertisement.Link));
        Assert.False(string.IsNullOrEmpty(advertisement.Url));
    }
    
    [Fact]
    public async Task AddAdvertisement_CreateNewAdversitement_ReturnsOkResult()
    {
        var contentFile = new MultipartFormDataContent();

        await using var fileStream = File.OpenRead(@"../../../Images/javascript-it-юмор-geek-5682739.jpeg");
        var fileContent = new StreamContent(fileStream);
        contentFile.Add(fileContent, "Image", "javascript-it-юмор-geek-5682739.jpeg");

        // Добавляем поля модели в форму
        contentFile.Add(new StringContent("https://dom-250.ru/1-season/1-sezon-7-seriya/"), "Link");
        contentFile.Add(new StringContent("15"), "Cost");
        contentFile.Add(new StringContent(DateTime.Today.ToString("yyyy-MM-dd")), "StartDate");
        contentFile.Add(new StringContent(DateTime.Today.ToString("yyyy-MM-dd")), "EndDate");

        var response = await _client.PostAsync("/api/Advertisement", contentFile);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task GetAllAdvertisement_Ok_ReturnsOkResult()
    {
        // Act
        var response =
            await _client.GetAsync($"/api/Advertisement/All");

        var content = await response.Content.ReadAsStringAsync();
        var advertisements = JsonConvert.DeserializeObject<ICollection<Advertisement>>(content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(advertisements);
        Assert.NotEmpty(advertisements);
    }

    private async Task<int> AddAsync()
    {
        var contentFile = new MultipartFormDataContent();

        await using var fileStream = File.OpenRead(@"../../../Images/javascript-it-юмор-geek-5682739.jpeg");
        var fileContent = new StreamContent(fileStream);
        contentFile.Add(fileContent, "Image", "javascript-it-юмор-geek-5682739.jpeg");

        // Добавляем поля модели в форму
        contentFile.Add(new StringContent("https://dom-250.ru/1-season/1-sezon-7-seriya/"), "Link");
        contentFile.Add(new StringContent("15"), "Cost");
        contentFile.Add(new StringContent(DateTime.Today.ToString("yyyy-MM-dd")), "StartDate");
        contentFile.Add(new StringContent(DateTime.Today.ToString("yyyy-MM-dd")), "EndDate");

        var response = await _client.PostAsync("/api/Advertisement", contentFile);
        var content = await response.Content.ReadAsStringAsync();
        return int.Parse(content);
    }
    
    [Fact]
    public async Task Delete_Ok_ReturnsOkResult()
    {
        int id = await AddAsync();
        
        // Act
        var response =
            await _client.DeleteAsync($"/api/Advertisement?id={id}");
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}