﻿using AuthorVerseServer.DTO;
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
        public GenreContollerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            var currentDirectory = Environment.CurrentDirectory;
            string path = Path.Combine(currentDirectory, "../../../");
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseSolutionRelativeContentRoot(path);
            });
        }

        [Fact]
        public async Task GetGenres_ReturnsOkResult()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

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
    }
}
