﻿using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Xunit;

namespace AuthorVerseServer.Tests.Integrations
{
    public class TagControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        WebApplicationFactory<Program> _factory;
        public TagControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetGenres_ReturnsOkResult()
        {
            HttpClient client = _factory.CreateClient();
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
    }
}