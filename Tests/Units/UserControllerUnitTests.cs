using AuthorVerseServer.Controllers;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace AuthorVerseServer.Tests.Units
{
    public class UserControllerUnitTests
    {
        private readonly Mock<IUser> _mockUser;
        private readonly UserController _userController;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<MailService> _mockEmailService;
        private readonly Mock<CreateJWTtokenService> _mockJWTTokenService;
        private readonly Mock<IDatabase> _redis;

        public UserControllerUnitTests()
        {
            _mockUser = new Mock<IUser>();
            _redis = new Mock<IDatabase>();
            var redisConnection = new Mock<IConnectionMultiplexer>();
            _mockUserManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _mockEmailService = new Mock<MailService>();
            _mockJWTTokenService = new Mock<CreateJWTtokenService>();

            var generateRandomName = new Mock<GenerateRandomName>();

            var databaseMock = new Mock<IDatabase>();
            redisConnection.Setup(mock => mock.GetDatabase(It.IsAny<int>(), null)).Returns(databaseMock.Object);

            _userController = new UserController(
                _mockUser.Object, redisConnection.Object, _mockUserManager.Object, _mockEmailService.Object, _mockJWTTokenService.Object, generateRandomName.Object);
        }

        [Fact]
        public async Task Registration_ExistingUserName_ReturnsBadRequest()
        {
            // Arrange
            _mockUserManager.Setup(um => um.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());

            _redis.Setup(um => um.StringGetAsync("user:existingUser", CommandFlags.None))
                .ReturnsAsync("someSerializedUserJson");

            // Act
            var result = await _userController.Registration(new UserRegistrationDTO { UserName = "existingUser" });

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var message = Assert.IsType<MessageDTO>(badRequestResult.Value);
            Assert.Equal("This name is already taken", message.message);
        }

        [Fact]
        public async Task Registration_ExistingEmail_ReturnsBadRequest()
        {
            // Arrange
            _redis.Setup(um => um.StringGetAsync($"user:{It.IsAny<string>}", CommandFlags.None))
                .ReturnsAsync("someSerializedUserJson");

            _mockUserManager.Setup(um => um.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            _mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());

            // Act
            var result = await _userController.Registration(new UserRegistrationDTO { UserName = "existingUser" });

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var message = Assert.IsType<MessageDTO>(badRequestResult.Value);
            Assert.Equal("This email is already taken", message.message);
        }

      
    }
}
