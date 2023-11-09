using AuthorVerseServer.Controllers;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AuthorVerseServer.Tests.Units
{
    public class CommentContollerUnitTests
    {
        [Fact]
        public async Task CreateComment_InvalidUserId_ShouldReturn()
        {
            // Arrange
            var mockCommentRepository = new Mock<IComment>();
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var controller = new CommentController(mockCommentRepository.Object, mockUserManager.Object);

            var commentDTO = new CreateCommentDTO
            {
                UserId = string.Empty,
                BookId = 0,
                Text = "Я и мой комментарий. Почти как Мама, папа я и бд, но только с комментарием",
            };

            // Act
            var result = await controller.CreateComment(commentDTO);
            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public async Task CreateComment_UserNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var mockCommentRepository = new Mock<IComment>();
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var controller = new CommentController(mockCommentRepository.Object, mockUserManager.Object);

            var commentDTO = new CreateCommentDTO
            {
                UserId = "Netu tacogo",
                BookId = 0,
                Text = "Я и мой комментарий. Почти как Мама, папа я и бд, но только с комментарием",
            };

            mockUserManager.Setup(um => um.FindByIdAsync(commentDTO.UserId)).ReturnsAsync((User?)null);
            mockCommentRepository.Setup(com => com.CheckUserComment(new Book(), new User())).ReturnsAsync(new Comment());

            // Act
            var result = await controller.CreateComment(commentDTO);
            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateComment_BookNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var mockCommentRepository = new Mock<IComment>();
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var controller = new CommentController(mockCommentRepository.Object, mockUserManager.Object);

            var commentDTO = new CreateCommentDTO
            {
                UserId = "admin",
                BookId = -5,
                Text = "Я и мой комментарий. Почти как Мама, папа я и бд, но только с комментарием",
            };

            mockUserManager.Setup(um => um.FindByIdAsync(commentDTO.UserId)).ReturnsAsync(new User());
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
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var controller = new CommentController(mockCommentRepository.Object, mockUserManager.Object);

            var commentDTO = new CreateCommentDTO
            {
                UserId = "admin",
                BookId = 0,
                Text = "Я и мой комментарий. Почти как Мама, папа я и бд, но только с комментарием",
            };

            mockUserManager.Setup(um => um.FindByIdAsync(commentDTO.UserId)).ReturnsAsync(new User());
            mockCommentRepository.Setup(com => com.GetBook(0)).ReturnsAsync(new Book());
            mockCommentRepository.Setup(com => com.CheckUserComment(new Book(), new User())).ReturnsAsync(new Comment());

            // Act
            var result = await controller.CreateComment(commentDTO);
            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateComment_AddComment_ShouldReturnOk()
        {
            // Arrange
            var mockCommentRepository = new Mock<IComment>();
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var controller = new CommentController(mockCommentRepository.Object, mockUserManager.Object);

            var commentDTO = new CreateCommentDTO
            {
                UserId = "admin",
                BookId = 0,
                Text = "Я и мой комментарий. Почти как Мама, папа я и бд, но только с комментарием",
            };

            mockUserManager.Setup(um => um.FindByIdAsync(commentDTO.UserId)).ReturnsAsync(new User());
            mockCommentRepository.Setup(com => com.GetBook(0)).ReturnsAsync(new Book());
            mockCommentRepository.Setup(com => com.CheckUserComment(new Book(), new User())).ReturnsAsync((Comment?)null);

            // Act
            var result = await controller.CreateComment(commentDTO);
            // Assert
            Assert.IsType<OkResult>(result.Result);
        }
    }
}
