using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Xunit;

namespace AuthorVerseServer.Tests.Integration
{
    public class TagControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        WebApplicationFactory<Program> _factory;
        string path = "C:\\Users\\buryy\\source\\repos\\AuthorVerseServer";
        public TagControllerIntegrationTests(WebApplicationFactory<Program> factory)
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
            var response = await client.GetAsync("/api/Tag");

            // Assert
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            ICollection<TagDTO>? tags = JsonConvert.DeserializeObject<ICollection<TagDTO>>(content);
            Assert.True(tags is not null, "Genres are null");
            Assert.True(tags.Count > 0, "Count of tags is zero");

            foreach (var tag in tags)
            {
                Assert.True(tag.TagId > 0, "Id is not correct");
                Assert.True(!string.IsNullOrEmpty(tag.Name), "Name not have");
            }
        }

        [Fact]
        public async Task GetTag_CacheSave_ReturnsOkResult()
        {
            var fakeTagService = new FakeTagService();

            HttpClient client = _factory.WithWebHostBuilder(builder =>
            {
                builder.UseSolutionRelativeContentRoot(path);
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton<ITag>(fakeTagService);
                });
            }).CreateClient();

            // Act
            var response = await client.GetAsync("/api/Tag");

            // Assert
            response.EnsureSuccessStatusCode();

            // Act
            var response2 = await client.GetAsync("/api/Tag");

            // Assert
            response2.EnsureSuccessStatusCode();

            Assert.Equal(1, fakeTagService.GetCallCount);
        }

        public class FakeTagService : ITag
        {
            public int GetCallCount { get; private set; }

            public Task AddTag(string name)
            {
                throw new NotImplementedException();
            }

            public async Task<ICollection<TagDTO>> GetTagAsync()
            {
                GetCallCount++;
                return new List<TagDTO> { new TagDTO { TagId = 1, Name = "YYY" } };
            }

            public Task Save()
            {
                throw new NotImplementedException();
            }
        }
    }
}
