using AuthorVerseServer.Controllers;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Enums;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using Xunit;

public class BookRepositoryTests
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
            AuthorId = "3f1dea02-0436-4570-8718-51596e4b2987",
            GenresId = new List<int> { 17, 18 },
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
            AuthorId = "3f1dea02-0436-4570-8718-51596e4b2987",
            GenresId = new List<int> { 999 }, 
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
    public async Task CreateBook_Ok_ShouldReturnOkRequest()
    {
        // Arrange
        var mockBookRepository = new Mock<IBook>();
        var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

        var controller = new BookController(mockBookRepository.Object, mockUserManager.Object);
        var bookDTO = new CreateBookDTO
        {
            AuthorId = "3f1dea02-0436-4570-8718-51596e4b2987",
            GenresId = new List<int> { 17, 18 },
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

        // Act
        var result = await controller.CreateBook(bookDTO);

        // Assert
        var objectResult = Assert.IsType<ActionResult<string>>(result);

        if (objectResult.Result is ObjectResult resultObject)
        {
            if (resultObject.StatusCode == 200)
            {
                Assert.Equal("Book created", resultObject.Value);
            }
            else if (resultObject.StatusCode == 400)
            {
                Assert.Equal("Invalid user Id", resultObject.Value);
            }
            else if (resultObject.StatusCode == 404)
            {
                Assert.Equal("Author not found", resultObject.Value);
            }
        }

        mockUserManager.Verify(um => um.FindByIdAsync(bookDTO.AuthorId), Times.Once);
        mockBookRepository.Verify(repo => repo.GetGenreById(17), Times.Once);
        mockBookRepository.Verify(repo => repo.GetGenreById(18), Times.Once);
        mockBookRepository.Verify(repo => repo.AddBookGenre(It.IsAny<BookGenre>()), Times.Exactly(2));
        mockBookRepository.Verify(repo => repo.Save(), Times.Once);

    }

}
