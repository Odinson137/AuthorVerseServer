using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Net;
using Xunit;

namespace AuthorVerseServer.Tests.Integrations
{
    public class UserControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        WebApplicationFactory<Program> _factory;
        private CreateJWTtokenService _createToken;

        string path = "C:\\Users\\buryy\\source\\repos\\AuthorVerseServer";
        public UserControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            var fakeConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Jwt:Key", "TOPSECRETKEY21231q43234" },
                    { "Jwt:Issuer", "AuthorVerseTest" }
                })
                .Build();

            _createToken = new CreateJWTtokenService(fakeConfiguration);
        }

        [Fact]
        public async Task Registration_ModelStateCheck_ReturnsBadRequest()
        {
            HttpClient client = _factory.WithWebHostBuilder(builder =>
            {
                builder.UseSolutionRelativeContentRoot(path);
            }).CreateClient();

            // Arrange
            var userDTO = new UserRegistrationDTO
            {
                UserName = "admin",
                Email = "buryy137@gmail.com",
                Password = "12345678",
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/User/Gmail", userDTO);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Registration_Ok_ReturnsOkResult()
        {
            HttpClient client = _factory.WithWebHostBuilder(builder =>
            {
                builder.UseSolutionRelativeContentRoot(path);
            }).CreateClient();

            // Arrange
            var userDTO = new UserRegistrationDTO
            {
                UserName = "admin",
                Email = "buryy137@gmail.com",
                Password = "12345678ASf!",
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/User/Gmail", userDTO);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task ConfirmEmail_Ok_ReturnsOkResult()
        {
            HttpClient client = _factory.WithWebHostBuilder(builder =>
            {
                builder.UseSolutionRelativeContentRoot(path);
            }).CreateClient();

            var generate = new GenerateRandomName();

            // Arrange
            var userDTO = new UserRegistrationDTO
            {
                UserName = generate.GenerateRandomUsername(),   
                Email = "testEmail@gmail.com",
                Password = "12345678ASf!",
            };

            var token = _createToken.GenerateJwtTokenEmail(userDTO);

            // Act
            var response = await client.PostAsync($"/api/User/EmailConfirm/{token}", new StringContent(""));

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Login_Ok_ReturnsOkResult()
        {
            HttpClient client = _factory.WithWebHostBuilder(builder =>
            {
                builder.UseSolutionRelativeContentRoot(path);
            }).CreateClient();

            var generate = new GenerateRandomName();

            // Arrange
            var userDTO = new UserLoginDTO
            {
                UserName = "Admin",
                Password = "Password@123",
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/User/Login", userDTO);
            var content = await response.Content.ReadAsStringAsync();
            // Assert
            response.EnsureSuccessStatusCode();
            var userVerify = JsonConvert.DeserializeObject<UserVerify>(content);
            Assert.NotNull(userVerify);
            Assert.IsType<UserVerify>(userVerify);
        }
    }

}
