using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AuthorVerseServer.Tests.Integrations
{
    public class GenreContollerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        WebApplicationFactory<Program> _factory;
        string path = "C:\\Users\\buryy\\source\\repos\\AuthorVerseServer";
        public GenreContollerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetGenres_ReturnsOkResult()
        {
            HttpClient client = _factory.WithWebHostBuilder(builder =>
             {
                 builder.UseSolutionRelativeContentRoot(path);
             }).CreateClient();

            // Arrange

            // Act
            var response = await client.GetAsync("/api/Genre");

            // Assert
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            ICollection<GenreDTO>? genres = JsonConvert.DeserializeObject<ICollection<GenreDTO>>(content);
            Assert.True(genres is not null, "Genres are null");
            Assert.True(genres.Count > 0, "Count of genres is zero");

            foreach (var genre in genres)
            {
                Assert.True(genre.GenreId > 0, "Id is not correct");
                Assert.True(!string.IsNullOrEmpty(genre.Name), "Name not have");
            }
        }

        [Fact]
        public async Task GetGenres_CacheSave_ReturnsOkResult()
        {
            var fakeGenreService = new FakeGenreService(); 

            HttpClient client = _factory.WithWebHostBuilder(builder =>
            {
                builder.UseSolutionRelativeContentRoot(path);
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton<IGenre>(fakeGenreService);
                });
            }).CreateClient();

            // Act
            var response = await client.GetAsync("/api/Genre");

            // Assert
            response.EnsureSuccessStatusCode();

            // Act
            var response2 = await client.GetAsync("/api/Genre");

            // Assert
            response2.EnsureSuccessStatusCode();

            Assert.Equal(1, fakeGenreService.GetGenreAsyncCallCount);
        }

        public class FakeGenreService : IGenre
        {
            public int GetGenreAsyncCallCount { get; private set; }

            public Task AddGenre(string name)
            {
                throw new NotImplementedException();
            }

            public async Task<ICollection<GenreDTO>> GetGenreAsync()
            {
                GetGenreAsyncCallCount++;
                return new List<GenreDTO> { new GenreDTO { GenreId = 1, Name = "YYY" } };
            }

            public Task Save()
            {
                throw new NotImplementedException();
            }
        }
    }
}
