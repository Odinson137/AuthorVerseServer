using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Net;
using Xunit;
using static AuthorVerseServer.Tests.Integrations.GenreContollerIntegrationTests;

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
        public async Task ConfirmEmail_FaildInCache_ReturnsBadRequest()
        {
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.UseSolutionRelativeContentRoot(path);
            }).CreateClient();

            var generate = new GenerateRandomName();

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
            var response = await client.PostAsync($"/api/User/EmailConfirm/{token}", new StringContent(""));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ConfirmEmail_Ok_ReturnsOkResult()
        {
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.UseSolutionRelativeContentRoot(path);
            }).CreateClient();

            var generate = new GenerateRandomName();

            // Arrange
            var userName = generate.GenerateRandomUsername();
            var userDTO = new UserRegistrationDTO
            {
                UserName = userName,   
                Email = "testEmail@gmail.com",
                Password = "12345678ASf!",
            };

            var response1 = await client.PostAsJsonAsync("/api/User/Gmail", userDTO);

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

    public class FakeCache : IMemoryCache
    {
        private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();

        public void Set(string key, object value)
        {
            _cache[key] = value;
        }

        public UserRegistrationDTO Get(string key)
        {
            if (_cache.TryGetValue(key, out var value) && value is UserRegistrationDTO result)
            {
                return result;
            }

            return default(UserRegistrationDTO);
        }

        public ICacheEntry CreateEntry(object key)
        {
            throw new NotImplementedException();
        }

        public void Remove(object key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(object key, out object? value)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }


}
