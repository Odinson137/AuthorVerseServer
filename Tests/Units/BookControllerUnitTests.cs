using AuthorVerseServer.Controllers;
using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class BookControllerUnitTests
{
    [Fact]
    public async Task CreateBook_EmptyAuthorId_ShouldReturnBadRequest()
    {
        // Arrange
        var mockBookRepository = new Mock<IBook>();
        var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

        var controller = new BookController(mockBookRepository.Object, mockUserManager.Object);

        var bookDTO = new CreateBookDTO
        {
            AuthorId = string.Empty,
            GenresId = new List<int> { 17, 18 },
            TagsId = new List<int>() { 1, 2 },
            Title = "Берсерк",
            Description = "Черный мечник идёт за тобой",
            AgeRating = AgeRating.EighteenPlus,
        };

        // Act
        var result = await controller.CreateBook(bookDTO);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateBook_EmptyBooksId_ShouldReturnBadRequest()
    {
        // Arrange
        var mockBookRepository = new Mock<IBook>();
        var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

        var controller = new BookController(mockBookRepository.Object, mockUserManager.Object);

        var bookDTO = new CreateBookDTO
        {
            AuthorId = "admin",
            GenresId = null,
            TagsId = new List<int>() { 1, 2 },
            Title = "Берсерк",
            Description = "Черный мечник идёт за тобой",
            AgeRating = AgeRating.EighteenPlus,
        };

        // Act
        var result = await controller.CreateBook(bookDTO);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateBook_ZeroSelectedGenres_ShouldReturnBadRequest()
    {
        // Arrange
        var mockBookRepository = new Mock<IBook>();
        var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

        var controller = new BookController(mockBookRepository.Object, mockUserManager.Object);

        var bookDTO = new CreateBookDTO
        {
            AuthorId = "admin",
            GenresId = new List<int>(),
            TagsId = new List<int>() { 1, 2 },
            Title = "Берсерк",
            Description = "Черный мечник идёт за тобой",
            AgeRating = AgeRating.EighteenPlus,
        };

        // Act
        var result = await controller.CreateBook(bookDTO);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateBook_EmptyTagsId_ShouldReturnBadRequest()
    {
        // Arrange
        var mockBookRepository = new Mock<IBook>();
        var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

        var controller = new BookController(mockBookRepository.Object, mockUserManager.Object);

        var bookDTO = new CreateBookDTO
        {
            AuthorId = "admin",
            GenresId = new List<int>() { 1, 2 },
            TagsId = null,
            Title = "Берсерк",
            Description = "Черный мечник идёт за тобой",
            AgeRating = AgeRating.EighteenPlus,
        };

        // Act
        var result = await controller.CreateBook(bookDTO);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateBook_ZeroSelectedTags_ShouldReturnBadRequest()
    {
        // Arrange
        var mockBookRepository = new Mock<IBook>();
        var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

        var controller = new BookController(mockBookRepository.Object, mockUserManager.Object);

        var bookDTO = new CreateBookDTO
        {
            AuthorId = "admin",
            GenresId = new List<int>() { 1, 2 },
            TagsId = new List<int>(),
            Title = "Берсерк",
            Description = "Черный мечник идёт за тобой",
            AgeRating = AgeRating.EighteenPlus,
        };

        // Act
        var result = await controller.CreateBook(bookDTO);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateBook_UserNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var mockBookRepository = new Mock<IBook>();
        var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

        var controller = new BookController(mockBookRepository.Object, mockUserManager.Object);

        var bookDTO = new CreateBookDTO
        {
            AuthorId = "admin",
            GenresId = new List<int> { 1, 2 },
            TagsId = new List<int> { 1, 2 },
            Title = "Берсерк",
            Description = "Черный мечник идёт за тобой",
            AgeRating = AgeRating.EighteenPlus,
        };
        mockUserManager.Setup(um => um.FindByIdAsync(bookDTO.AuthorId)).ReturnsAsync((User)null);

        // Act
        var result = await controller.CreateBook(bookDTO);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateBook_InvalidGenres_ShouldReturnNotFound()
    {
        // Arrange
        var mockBookRepository = new Mock<IBook>();
        var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

        var controller = new BookController(mockBookRepository.Object, mockUserManager.Object);

        var bookDTO = new CreateBookDTO
        {
            AuthorId = "admin",
            GenresId = new List<int> { 999 },
            TagsId = new List<int> { 1, 2 },
            Title = "Берсерк",
            Description = "Черный мечник идёт за тобой",
            AgeRating = AgeRating.EighteenPlus,
        };

        // Setup mock Genre objects
        var genre1 = new Genre { GenreId = 17, Name = "Фантастика" };
        mockBookRepository.Setup(repo => repo.GetGenreById(It.IsAny<int>()))
                         .ReturnsAsync((int genreId) => genreId == 17 ? genre1 : null);

        // Act
        var result = await controller.CreateBook(bookDTO);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateBook_InvalidTags_ShouldReturnNotFound()
    {
        // Arrange
        var mockBookRepository = new Mock<IBook>();
        var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

        var controller = new BookController(mockBookRepository.Object, mockUserManager.Object);

        var bookDTO = new CreateBookDTO
        {
            AuthorId = "admin",
            GenresId = new List<int> { 17 },
            TagsId = new List<int> { 1, 2 },
            Title = "Берсерк",
            Description = "Черный мечник идёт за тобой",
            AgeRating = AgeRating.EighteenPlus,
        };

        var genre1 = new Genre { GenreId = 17, Name = "Фантастика" };
        var genre2 = new Genre { GenreId = 18, Name = "Детектив" };
        mockBookRepository.Setup(repo => repo.GetGenreById(It.IsAny<int>()))
                         .ReturnsAsync((int genreId) => genreId == 17 ? genre1 : genre2);

        var tag = new Tag { TagId = 1, Name = "Книга" };
        mockBookRepository.Setup(repo => repo.GetTagById(It.IsAny<int>()))
                    .ReturnsAsync((int tagId) => tagId == 1 ? tag : null);

        // Act
        var result = await controller.CreateBook(bookDTO);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateBook_Ok_ShouldReturnOkRequest()
    {
        // Arrange
        var mockBookRepository = new Mock<IBook>();
        var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

        var controller = new BookController(mockBookRepository.Object, mockUserManager.Object);
        var bookDTO = new CreateBookDTO
        {
            AuthorId = "admin",
            GenresId = new List<int> { 17, 18 },
            TagsId = new List<int> { 1, 2 },
            Title = "Берсерк",
            Description = "Черный мечник идёт за тобой",
            AgeRating = AgeRating.EighteenPlus,
        };

        // Setup mock User object
        mockUserManager.Setup(um => um.FindByIdAsync(bookDTO.AuthorId)).ReturnsAsync(new User());

        // Setup mock Genre objects
        var genre1 = new Genre { GenreId = 17, Name = "Фантастика" };
        var genre2 = new Genre { GenreId = 18, Name = "Детектив" };
        mockBookRepository.Setup(repo => repo.GetGenreById(It.IsAny<int>()))
                         .ReturnsAsync((int genreId) => genreId == 17 ? genre1 : genre2);

        var tag1 = new Tag { TagId = 1, Name = "Книга" };
        var tag2 = new Tag { TagId = 2, Name = "Аудиокнига" };
        mockBookRepository.Setup(repo => repo.GetTagById(It.IsAny<int>()))
                    .ReturnsAsync((int tagId) => tagId == 1 ? tag1 : tag2);

        // Act
        var result = await controller.CreateBook(bookDTO);

        // Assert
        var objectResult = Assert.IsType<ActionResult<int>>(result);
        Assert.IsType<OkObjectResult>(result.Result);

        mockUserManager.Verify(um => um.FindByIdAsync(bookDTO.AuthorId), Times.Once);
        mockBookRepository.Verify(repo => repo.GetGenreById(17), Times.Once);
        mockBookRepository.Verify(repo => repo.GetGenreById(18), Times.Once);
        mockBookRepository.Verify(repo => repo.GetTagById(1), Times.Once);
        mockBookRepository.Verify(repo => repo.GetTagById(2), Times.Once);
        mockBookRepository.Verify(repo => repo.Save(), Times.Once);
    }

}
