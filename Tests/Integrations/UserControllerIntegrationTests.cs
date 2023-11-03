using AuthorVerseServer.DTO;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using Xunit;

namespace AuthorVerseServer.Tests.Integrations
{
    public class UserControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        WebApplicationFactory<Program> _factory;
        private CreateJWTtokenService _createToken;


        string path;

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

            var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            //Для выделения пути к каталогу, воспользуйтесь `System.IO.Path`:
            path = GetBinFolderPath(location);


            _createToken = new CreateJWTtokenService(fakeConfiguration);
        }

        static string GetBinFolderPath(string fullPath)
        {
            string binFolderName = "bin";
            string directorySeparator = Path.DirectorySeparatorChar.ToString();

            // Получаем индекс последнего вхождения папки "bin" в полный путь
            int index = fullPath.LastIndexOf(directorySeparator + binFolderName, StringComparison.OrdinalIgnoreCase);

            // Если папка "bin" найдена в пути, обрезаем путь до этой папки
            if (index >= 0)
            {
                // Добавляем 1, чтобы включить разделитель директории в обрезанном пути
                return fullPath.Substring(0, index + 1);
            }

            // Если папка "bin" не найдена, возвращаем пустую строку или обрабатываем ошибку, как требуется
            return string.Empty;
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
}
