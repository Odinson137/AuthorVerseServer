using AuthorVerseServer.Controllers;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ServerTests.Units;

public class CommentContollerUnitTests
{
    public readonly Mock<IComment> mockCommentRepository;
    public readonly Mock<CreateJWTtokenService> mockTokenService;
    public readonly Mock<UserManager<User>> mockUserManager;
    public readonly CommentController controller;
    public CommentContollerUnitTests()
    {
        mockCommentRepository = new Mock<IComment>();
        mockTokenService = new Mock<CreateJWTtokenService>();
        mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        
        controller = new CommentController(mockCommentRepository.Object, mockUserManager.Object, mockTokenService.Object);
    }

    [Fact]
    public async Task CreateComment_UserNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var commentDTO = new CreateCommentDTO
        {
            BookId = 0,
            Text = "Я и мой комментарий. Почти как Мама, папа я и бд, но только с комментарием",
        };

        mockTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");
        mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        mockCommentRepository.Setup(com => com.CheckExistCommentAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(true);

        // Act
        var result = await controller.CreateComment(commentDTO);
        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateComment_BookNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var commentDTO = new CreateCommentDTO
        {
            BookId = -5,
            Text = "Я и мой комментарий. Почти как Мама, папа я и бд, но только с комментарием",
        };

        mockTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");
        mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
        mockCommentRepository.Setup(com => com.ChechExistBookAsync(-5)).ReturnsAsync(0);

        // Act
        var result = await controller.CreateComment(commentDTO);
        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateComment_CommentAlreadyExist_ShouldReturnBadRequest()
    {
        // Arrange

        var commentDTO = new CreateCommentDTO
        {
            BookId = 0,
            Text = "Я и мой комментарий. Почти как Мама, папа я и бд, но только с комментарием",
        };

        mockTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");
        mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
        mockCommentRepository.Setup(com => com.ChechExistBookAsync(commentDTO.BookId)).ReturnsAsync(1);
        mockCommentRepository.Setup(com => com.CheckExistCommentAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(true);

        // Act
        var result = await controller.CreateComment(commentDTO);
        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateComment_AddComment_ShouldReturnOk()
    {
        // Arrange

        var commentDTO = new CreateCommentDTO
        {
            BookId = 0,
            Text = "Я и мой комментарий. Почти как Мама, папа я и бд, но только с комментарием",
        };
        mockTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");

        mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
        mockCommentRepository.Setup(com => com.ChechExistBookAsync(commentDTO.BookId)).ReturnsAsync(1);
        mockCommentRepository.Setup(com => com.CheckExistCommentAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(false);

        // Act
        var result = await controller.CreateComment(commentDTO);
        // Assert
        Assert.IsType<OkResult>(result.Result);
    }


    [Fact]
    public async Task DeleteComment_CommentNotFound_ShouldReturnNotFound()
    {
        // Arrange

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
    public async Task DeleteComment_Ok_ShouldReturnOkResult()
    {
        // Arrange

        mockCommentRepository.Setup(com => com.GetCommentAsync(It.IsAny<int>())).ReturnsAsync(new Comment());

        mockTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal?>())).Returns("admin");

        mockCommentRepository.Setup(com => com.DeleteComment(It.IsAny<int>()));
        mockCommentRepository.Setup(com => com.CheckExistCommentAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(true);

        // Act
        var result = await controller.DeleteComment(1);
        // Assert
        Assert.IsType<OkResult>(result.Result);
    }

    [Fact]
    public async Task ChangeCommentText_CommentNotFound_ShouldReturnNotFound()
    {
        // Arrange

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

        mockCommentRepository.Setup(com => com.GetCommentAsync(It.IsAny<int>())).ReturnsAsync(new Comment());
        mockTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal?>())).Returns("admin");

        mockCommentRepository.Setup(com => com.Save()).ReturnsAsync(0);

        // Act
        var result = await controller.ChangeComment(1, "new longer text dfgggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggg");
        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task ChangeCommentText_CommentChanged_ShouldReturnOkResult()
    {
        // Arrange

        mockCommentRepository.Setup(com => com.GetCommentAsync(It.IsAny<int>())).ReturnsAsync(new Comment());
        mockTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");

        mockCommentRepository.Setup(com => com.Save()).ReturnsAsync(1);

        // Act
        var result = await controller.ChangeComment(1, "new dgffffffffffffff longer sdfffffffffffffffddf text must contains 50 letters or more");
        // Assert
        Assert.IsType<OkResult>(result.Result);
    }

    //[Fact]
    //public async Task ChangeUpRating_CommentNotFound_ShouldReturnNotFound()
    //{
    //    // Arrange

    //    mockCommentRepository.Setup(com => com.GetCommentAsync(It.IsAny<int>())).ReturnsAsync((Comment?)null);
    //    mockTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal?>())).Returns("admin");

    //    mockCommentRepository.Setup(com => com.Save()).ReturnsAsync(1);

    //    // Act
    //    var result = await controller.ChangeUpRating(1);
    //    // Assert
    //    Assert.IsType<NotFoundObjectResult>(result.Result);
    //}

    //[Fact]
    //public async Task ChangeUpRating_CommentNotUpRating_ShouldReturnBadRequest()
    //{
    //    // Arrange

    //    mockCommentRepository.Setup(com => com.GetCommentAsync(It.IsAny<int>())).ReturnsAsync(new Comment());
    //    mockTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal?>())).Returns("admin");

    //    mockCommentRepository.Setup(com => com.Save()).ReturnsAsync(0);

    //    // Act
    //    var result = await controller.ChangeUpRating(1);
    //    // Assert
    //    Assert.IsType<BadRequestObjectResult>(result.Result);
    //}

    //[Fact]
    //public async Task ChangeUpRating_CommentUpRating_ShouldReturnOkResult()
    //{
    //    // Arrange

    //    mockCommentRepository.Setup(com => com.GetCommentAsync(It.IsAny<int>())).ReturnsAsync(new Comment());
    //    mockTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal?>())).Returns("admin");

    //    mockCommentRepository.Setup(com => com.Save()).ReturnsAsync(1);

    //    // Act
    //    var result = await controller.ChangeUpRating(1);
    //    // Assert
    //    Assert.IsType<OkResult>(result.Result);
    //}


    //[Fact]
    //public async Task ChangeDownRating_CommentNotFound_ShouldReturnNotFound()
    //{
    //    // Arrange

    //    mockCommentRepository.Setup(com => com.GetCommentAsync(It.IsAny<int>())).ReturnsAsync((Comment?)null);
    //    mockTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal?>())).Returns("admin");

    //    mockCommentRepository.Setup(com => com.Save()).ReturnsAsync(1);

    //    // Act
    //    var result = await controller.ChangeDownRating(1);
    //    // Assert
    //    Assert.IsType<NotFoundObjectResult>(result.Result);
    //}

    //[Fact]
    //public async Task ChangeDownRating_CommentNotDownRating_ShouldReturnBadRequest()
    //{
    //    // Arrange

    //    mockCommentRepository.Setup(com => com.GetCommentAsync(It.IsAny<int>())).ReturnsAsync(new Comment());
    //    mockTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal?>())).Returns("admin");

    //    mockCommentRepository.Setup(com => com.Save()).ReturnsAsync(0);

    //    // Act
    //    var result = await controller.ChangeDownRating(1);
    //    // Assert
    //    Assert.IsType<BadRequestObjectResult>(result.Result);
    //}

    //[Fact]
    //public async Task ChangeDownRating_CommentDownRating_ShouldReturnOkResult()
    //{
    //    // Arrange

    //    mockCommentRepository.Setup(com => com.GetCommentAsync(It.IsAny<int>())).ReturnsAsync(new Comment());
    //    mockTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal?>())).Returns("admin");

    //    mockCommentRepository.Setup(com => com.Save()).ReturnsAsync(1);

    //    // Act
    //    var result = await controller.ChangeDownRating(1);
    //    // Assert
    //    Assert.IsType<OkResult>(result.Result);
    //}
}
