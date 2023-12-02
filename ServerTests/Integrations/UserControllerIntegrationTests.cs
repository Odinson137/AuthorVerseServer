using AuthorVerseServer.DTO;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;


namespace Server.Tests.Integrations;

public class UserControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private CreateJWTtokenService _createToken;
    private readonly HttpClient _client;

    public UserControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        var configuration = factory.Services.GetRequiredService<IConfiguration>();

        _createToken = new CreateJWTtokenService(configuration);
        _client = factory.CreateClient();

    }

    [Fact]
    public async Task Registration_ModelStateCheck_ReturnsBadRequest()
    {
        // Arrange
        var userDTO = new UserRegistrationDTO
        {
            UserName = "admin",
            Email = "buryy137@gmail.com",
            Password = "12345678",
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/User/Gmail", userDTO);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Registration_Ok_ReturnsOkResult()
    {
        // Arrange
        var userDTO = new UserRegistrationDTO
        {
            UserName = "admin",
            Email = "buryy137@gmail.com",
            Password = "12345678ASf!",
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/User/Gmail", userDTO);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task ConfirmEmail_FaildInCache_ReturnsBadRequest()
    {
        var generate = new GenerateRandomNameService();

        // Arrange
        var userName = generate.GenerateRandomUsername();
        var userDTO = new UserRegistrationDTO
        {
            UserName = userName,
            Email = "testEmail@gmail.com",
            Password = "12345678ASf!",
        };

        var token = _createToken.GenerateJwtTokenEmail(userDTO);
        // Act
        var response = await _client.PostAsync($"/api/User/EmailConfirm/{token}", new StringContent(""));

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task ConfirmEmail_Ok_ReturnsOkResult()
    {
        var generate = new GenerateRandomNameService();

        // Arrange
        var userName = generate.GenerateRandomUsername();
        var userDTO = new UserRegistrationDTO
        {
            UserName = userName,   
            Email = "testEmail@gmail.com",
            Password = "12345678ASf!",
        };

        var response1 = await _client.PostAsJsonAsync("/api/User/Gmail", userDTO);

        var token = _createToken.GenerateJwtTokenEmail(userDTO);
        // Act
        var response = await _client.PostAsync($"/api/User/EmailConfirm/{token}", new StringContent(""));

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Login_Ok_ReturnsOkResult()
    {
        var generate = new GenerateRandomNameService();

        // Arrange
        var userDTO = new UserLoginDTO
        {
            UserName = "Admin",
            Password = "Password@123",
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/User/Login", userDTO);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.False(string.IsNullOrEmpty(content));

        var handler = new JwtSecurityTokenHandler();
        string userId = handler.ReadJwtToken(content).Claims.First().Value;
        Assert.Equal(userDTO.UserName.ToLower(), userId.ToLower());
    }
}
