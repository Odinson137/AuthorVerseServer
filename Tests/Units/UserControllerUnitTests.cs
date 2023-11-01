using AuthorVerseServer.Controllers;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
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

        public UserControllerUnitTests()
        {
            _mockUser = new Mock<IUser>();
            _mockUserManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _mockEmailService = new Mock<MailService>();
            _mockJWTTokenService = new Mock<CreateJWTtokenService>();

            var generateRandomName = new Mock<GenerateRandomName>();

            _userController = new UserController(
                _mockUser.Object, _mockUserManager.Object, _mockEmailService.Object, _mockJWTTokenService.Object, generateRandomName.Object);
        }

        [Fact]
        public async Task Registration_ExistingUserName_ReturnsBadRequest()
        {
            // Arrange
            _mockUserManager.Setup(um => um.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());

            // Act
            var result = await _userController.Registration(new UserRegistrationDTO { UserName = "existingUser" });

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var message = Assert.IsType<MessageDTO>(badRequestResult.Value);
            Assert.Equal("This name is already taken", message.message);
        }

        [Fact]
        public async Task Registration_Ok_ReturnsOk()
        {
            // Arrange
            _mockUserManager.Setup(um => um.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            string token = "token";
            _mockJWTTokenService.Setup(um => um.GenerateJwtTokenEmail(new UserRegistrationDTO()))
                .Returns(token);

            _mockEmailService.Setup(um => um.SendEmail(token, "Yura"))
                .ReturnsAsync("Ok");

            // Act
            var result = await _userController.Registration(new UserRegistrationDTO());

            // Assert
            var okRequestResult = Assert.IsType<OkObjectResult>(result.Result);
        }
    }
}
