using AuthorVerseServer.Controllers;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace AuthorVerseServer.Tests.Units
{
    public class CommentContollerUnitTests
    {
        [Fact]
        public async Task CreateComment_UserNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var mockCommentRepository = new Mock<IComment>();
            var mockTokenService = new Mock<CreateJWTtokenService>();
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var controller = new CommentController(mockCommentRepository.Object, mockUserManager.Object, mockTokenService.Object);

            var commentDTO = new CreateCommentDTO
            {
                BookId = 0,
                Text = "Я и мой комментарий. Почти как Мама, папа я и бд, но только с комментарием",
            };

            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            mockCommentRepository.Setup(com => com.CheckUserComment(new Book(), new User())).ReturnsAsync(new Comment());

            // Act
            var result = await controller.CreateComment(commentDTO);
            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateComment_BookNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var mockCommentRepository = new Mock<IComment>();
            var mockTokenService = new Mock<CreateJWTtokenService>();
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var controller = new CommentController(mockCommentRepository.Object, mockUserManager.Object, mockTokenService.Object);

            var commentDTO = new CreateCommentDTO
            {
                BookId = -5,
                Text = "Я и мой комментарий. Почти как Мама, папа я и бд, но только с комментарием",
            };

            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
            mockCommentRepository.Setup(com => com.GetBook(-5)).ReturnsAsync((Book?)null);
            //mockCommentRepository.Setup(com => com.CheckUserComment(new Book(), new User())).ReturnsAsync(new Comment());

            // Act
            var result = await controller.CreateComment(commentDTO);
            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateComment_CommentAlreadyExist_ShouldReturnBadRequest()
        {
            // Arrange
            var mockCommentRepository = new Mock<IComment>();
            var mockTokenService = new Mock<CreateJWTtokenService>();
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var controller = new CommentController(mockCommentRepository.Object, mockUserManager.Object, mockTokenService.Object);

            var commentDTO = new CreateCommentDTO
            {
                BookId = 0,
                Text = "Я и мой комментарий. Почти как Мама, папа я и бд, но только с комментарием",
            };

            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
            mockCommentRepository.Setup(com => com.GetBook(0)).ReturnsAsync(new Book());
            mockCommentRepository.Setup(com => com.CheckUserComment(It.IsAny<Book>(), It.IsAny<User>())).ReturnsAsync(new Comment());

            // Act
            var result = await controller.CreateComment(commentDTO);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateComment_AddComment_ShouldReturnOk()
        {
            // Arrange
            var mockCommentRepository = new Mock<IComment>();
            var mockTokenService = new Mock<CreateJWTtokenService>();
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var controller = new CommentController(mockCommentRepository.Object, mockUserManager.Object, mockTokenService.Object);

            var commentDTO = new CreateCommentDTO
            {
                BookId = 0,
                Text = "Я и мой комментарий. Почти как Мама, папа я и бд, но только с комментарием",
            };

            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
            mockCommentRepository.Setup(com => com.GetBook(0)).ReturnsAsync(new Book());
            mockCommentRepository.Setup(com => com.CheckUserComment(new Book(), new User())).ReturnsAsync((Comment?)null);

            // Act
            var result = await controller.CreateComment(commentDTO);
            // Assert
            Assert.IsType<OkResult>(result.Result);
        }


        [Fact]
        public async Task DeleteComment_CommentNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var mockCommentRepository = new Mock<IComment>();
            var mockTokenService = new Mock<CreateJWTtokenService>();
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var controller = new CommentController(mockCommentRepository.Object, mockUserManager.Object, mockTokenService.Object);

            mockTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");
            mockCommentRepository.Setup(com => com.GetCommentAsync(It.IsAny<int>())).ReturnsAsync((Comment?)null);
            mockCommentRepository.Setup(com => com.Save());

            var claim = new Claim(JwtRegisteredClaimNames.Sub, "admin");

            // Act
            var result = await controller.DeleteComment(1);
            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteComment_CommentNotDeleted_ShouldReturnBadRequest()
        {
            // Arrange
            var mockCommentRepository = new Mock<IComment>();
            var mockTokenService = new Mock<CreateJWTtokenService>();
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var controller = new CommentController(mockCommentRepository.Object, mockUserManager.Object, mockTokenService.Object);

            mockCommentRepository.Setup(com => com.GetCommentAsync(It.IsAny<int>())).ReturnsAsync(new Comment());

            //var claim = new Claim(JwtRegisteredClaimNames.Sub, "admin");
            mockTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");

            mockCommentRepository.Setup(com => com.DeleteComment(It.IsAny<Comment>()));

            mockCommentRepository.Setup(com => com.Save()).ReturnsAsync(0);

            // Act
            var result = await controller.DeleteComment(1);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteComment_Ok_ShouldReturnOkResult()
        {
            // Arrange
            var mockCommentRepository = new Mock<IComment>();
            var mockTokenService = new Mock<CreateJWTtokenService>();
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var controller = new CommentController(mockCommentRepository.Object, mockUserManager.Object, mockTokenService.Object);

            mockCommentRepository.Setup(com => com.GetCommentAsync(It.IsAny<int>())).ReturnsAsync(new Comment());

            mockTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal?>())).Returns("admin");

            mockCommentRepository.Setup(com => com.DeleteComment(It.IsAny<Comment>()));

            mockCommentRepository.Setup(com => com.Save()).ReturnsAsync(1);

            // Act
            var result = await controller.DeleteComment(1);
            // Assert
            Assert.IsType<OkResult>(result.Result);
        }

        [Fact]
        public async Task ChangeCommentText_CommentNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var mockCommentRepository = new Mock<IComment>();
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockTokenService = new Mock<CreateJWTtokenService>();
            var controller = new CommentController(mockCommentRepository.Object, mockUserManager.Object, mockTokenService.Object);

            mockCommentRepository.Setup(com => com.GetCommentAsync(It.IsAny<int>())).ReturnsAsync((Comment?)null);
            mockTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal?>())).Returns("admin");

            mockCommentRepository.Setup(com => com.Save()).ReturnsAsync(1);

            // Act
            var result = await controller.ChangeComment(1, "new longer text dgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgfgf");
            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task ChangeCommentText_CommentNotChanged_ShouldReturnBadRequest()
        {
            // Arrange
            var mockCommentRepository = new Mock<IComment>();
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockTokenService = new Mock<CreateJWTtokenService>();
            var controller = new CommentController(mockCommentRepository.Object, mockUserManager.Object, mockTokenService.Object);

            mockCommentRepository.Setup(com => com.GetCommentAsync(It.IsAny<int>())).ReturnsAsync(new Comment());
            mockTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal?>())).Returns("admin");

            mockCommentRepository.Setup(com => com.Save()).ReturnsAsync(0);

            // Act
            var result = await controller.ChangeComment(1, "new longer text dfgggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggg");
            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task ChangeCommentText_CommentChanged_ShouldReturnOkResult()
        {
            // Arrange
            var mockCommentRepository = new Mock<IComment>();
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockTokenService = new Mock<CreateJWTtokenService>();
            var controller = new CommentController(mockCommentRepository.Object, mockUserManager.Object, mockTokenService.Object);

            mockCommentRepository.Setup(com => com.GetCommentAsync(It.IsAny<int>())).ReturnsAsync(new Comment());
            mockTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal?>())).Returns("admin");

            mockCommentRepository.Setup(com => com.Save()).ReturnsAsync(1);

            // Act
            var result = await controller.ChangeComment(1, "new dgffffffffffffff longer sdfffffffffffffffddf text must contains 50 letters or more");
            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task ChangeUpRating_CommentNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var mockCommentRepository = new Mock<IComment>();
            var mockTokenService = new Mock<CreateJWTtokenService>();
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var controller = new CommentController(mockCommentRepository.Object, mockUserManager.Object, mockTokenService.Object);

            mockCommentRepository.Setup(com => com.GetCommentAsync(It.IsAny<int>())).ReturnsAsync((Comment?)null);
            mockTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal?>())).Returns("admin");

            mockCommentRepository.Setup(com => com.Save()).ReturnsAsync(1);

            // Act
            var result = await controller.ChangeUpRating(1);
            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task ChangeUpRating_CommentNotUpRating_ShouldReturnBadRequest()
        {
            // Arrange
            var mockCommentRepository = new Mock<IComment>();
            var mockTokenService = new Mock<CreateJWTtokenService>();
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var controller = new CommentController(mockCommentRepository.Object, mockUserManager.Object, mockTokenService.Object);

            mockCommentRepository.Setup(com => com.GetCommentAsync(It.IsAny<int>())).ReturnsAsync(new Comment());
            mockTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal?>())).Returns("admin");

            mockCommentRepository.Setup(com => com.Save()).ReturnsAsync(0);

            // Act
            var result = await controller.ChangeUpRating(1);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task ChangeUpRating_CommentUpRating_ShouldReturnOkResult()
        {
            // Arrange
            var mockCommentRepository = new Mock<IComment>();
            var mockTokenService = new Mock<CreateJWTtokenService>();
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var controller = new CommentController(mockCommentRepository.Object, mockUserManager.Object, mockTokenService.Object);

            mockCommentRepository.Setup(com => com.GetCommentAsync(It.IsAny<int>())).ReturnsAsync(new Comment());
            mockTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal?>())).Returns("admin");

            mockCommentRepository.Setup(com => com.Save()).ReturnsAsync(1);

            // Act
            var result = await controller.ChangeUpRating(1);
            // Assert
            Assert.IsType<OkResult>(result.Result);
        }


        [Fact]
        public async Task ChangeDownRating_CommentNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var mockCommentRepository = new Mock<IComment>();
            var mockTokenService = new Mock<CreateJWTtokenService>();
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var controller = new CommentController(mockCommentRepository.Object, mockUserManager.Object, mockTokenService.Object);

            mockCommentRepository.Setup(com => com.GetCommentAsync(It.IsAny<int>())).ReturnsAsync((Comment?)null);
            mockTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal?>())).Returns("admin");

            mockCommentRepository.Setup(com => com.Save()).ReturnsAsync(1);

            // Act
            var result = await controller.ChangeDownRating(1);
            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task ChangeDownRating_CommentNotDownRating_ShouldReturnBadRequest()
        {
            // Arrange
            var mockCommentRepository = new Mock<IComment>();
            var mockTokenService = new Mock<CreateJWTtokenService>();
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var controller = new CommentController(mockCommentRepository.Object, mockUserManager.Object, mockTokenService.Object);

            mockCommentRepository.Setup(com => com.GetCommentAsync(It.IsAny<int>())).ReturnsAsync(new Comment());
            mockTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal?>())).Returns("admin");

            mockCommentRepository.Setup(com => com.Save()).ReturnsAsync(0);

            // Act
            var result = await controller.ChangeDownRating(1);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task ChangeDownRating_CommentDownRating_ShouldReturnOkResult()
        {
            // Arrange
            var mockCommentRepository = new Mock<IComment>();
            var mockTokenService = new Mock<CreateJWTtokenService>();
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var controller = new CommentController(mockCommentRepository.Object, mockUserManager.Object, mockTokenService.Object);

            mockCommentRepository.Setup(com => com.GetCommentAsync(It.IsAny<int>())).ReturnsAsync(new Comment());
            mockTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal?>())).Returns("admin");

            mockCommentRepository.Setup(com => com.Save()).ReturnsAsync(1);

            // Act
            var result = await controller.ChangeDownRating(1);
            // Assert
            Assert.IsType<OkResult>(result.Result);
        }
    }
}
