using AuthorVerseServer.Controllers;
using AuthorVerseServer.Interfaces.ServiceInterfaces;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Identity;
using Moq;
using StackExchange.Redis;

namespace ServerTests.Units;

public class QuoteControllerUnitTests
{
    readonly Mock<IBook> mockBookRepository;
    readonly Mock<UserManager<User>> mockUserManager;
    readonly Mock<ILoadImage> mockLoadImage;
    readonly BookController controller;
    private readonly Mock<IDatabase> _redis;
    public QuoteControllerUnitTests()
    {
        mockBookRepository = new Mock<IBook>();
        mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        mockLoadImage = new Mock<ILoadImage>();

        var redisConnection = new Mock<IConnectionMultiplexer>();

        _redis = new Mock<IDatabase>();
        redisConnection.Setup(mock => mock.GetDatabase(It.IsAny<int>(), null)).Returns(_redis.Object);
    }
}
