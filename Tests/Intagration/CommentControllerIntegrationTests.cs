﻿using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AuthorVerseServer.Tests.Integration
{
    public class CommentControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public CommentControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.UseSolutionRelativeContentRoot("C:\\Users\\buryy\\source\\repos\\AuthorVerseServer");
            }).CreateClient();
        }

        [Fact]
        public async Task CreateComment_ReturnsOkResult()
        {
            // Arrange
            var bookDTO = new CreateCommentDTO
            {
                UserId = "admin",
                BookId = 0,
                Text = "Я и мой комментарий. Почти как Мама, папа я и бд, но только с комментарием",
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Comment/Create", bookDTO);

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
