using AuthorVerseServer.Controllers;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTests.Units
{
    public class ForumMessageUnitTests
    {
        readonly Mock<IForumMessage> _mockForumMessage;
        readonly Mock<IDatabase> _redis;
        readonly ForumMessageController controller;

        public ForumMessageUnitTests()
        {
            _redis = new Mock<IDatabase>();
            var connection = new Mock<IConnectionMultiplexer>();
            _mockForumMessage = new Mock<IForumMessage>();
            controller = new ForumMessageController(_mockForumMessage.Object, connection.Object);
        }

        [Fact]
        public async Task AddMessage_ValueInRedisNotFound_ShouldReturnNotFound()
        {
            // Arrange
            string key = "key";

            _redis.Setup(cl => cl.StringGetAsync($"add_message:{It.IsAny<string>()}", CommandFlags.None)).ReturnsAsync(string.Empty);
            _mockForumMessage.Setup(cl => cl.AddForumMessageAsync(It.IsAny<ForumMessage>()));
            _mockForumMessage.Setup(cl => cl.SaveAsync());

            // Act
            var result = await controller.AddMessage(key);
            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        //[Fact]
        //public async Task AddMessage_ValuesAreNotDeserialize_ShouldReturnBadRequest()
        //{
        //    // Arrange
        //    string key = "key";

        //    _redis.Setup(cl => cl.StringGetAsync($"message:{key}", CommandFlags.None)).ReturnsAsync("value");
        //    _mockForumMessage.Setup(cl => cl.AddForumMessageAsync(It.IsAny<ForumMessage>()));
        //    _mockForumMessage.Setup(cl => cl.SaveAsync());

        //    // Act
        //    var result = await controller.AddMessage(key);
        //    // Assert
        //    Assert.IsType<BadRequestObjectResult>(result.Result);
        //}

        [Fact]
        public async Task AddMessage_Ok_ShouldReturnOk()
        {
            // Arrange
            string key = "key";

            string value = JsonConvert.SerializeObject(new SendForumMessageDTO() { AnswerId = 0, Text = "", UserId = "", BookId = 1});

            _redis.Setup(cl => cl.StringGetAsync($"add_message:{key}", CommandFlags.None)).ReturnsAsync(value);
            _mockForumMessage.Setup(cl => cl.AddForumMessageAsync(It.IsAny<ForumMessage>()));
            _mockForumMessage.Setup(cl => cl.SaveAsync());

            // Act
            var result = await controller.AddMessage(key);
            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }
    }
}
