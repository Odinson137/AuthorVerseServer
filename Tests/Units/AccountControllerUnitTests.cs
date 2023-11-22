using AuthorVerseServer.Controllers;
using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System.Security.Claims;
using Xunit;

namespace AuthorVerseServer.Tests.Units
{
    public class AccountControllerUnitTests
    {
        private readonly Mock<IAccount> _mockAccount;
        private readonly AccountController _accountController;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<MailService> _mockEmailService;
        private readonly Mock<CreateJWTtokenService> _mockJWTTokenService;
        private readonly Mock<IMemoryCache> _mockCache;

        public AccountControllerUnitTests()
        {
            _mockAccount = new Mock<IAccount>();
            _mockCache = new Mock<IMemoryCache>();
            _mockUserManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _mockEmailService = new Mock<MailService>();
            _mockJWTTokenService = new Mock<CreateJWTtokenService>();

            _accountController = new AccountController(
                _mockAccount.Object, _mockUserManager.Object, _mockEmailService.Object, _mockJWTTokenService.Object, _mockCache.Object);
        }

        [Fact]
        public async Task GetUserProfile_InvalidToken_ShouldReturnBadRequest()
        {
            // Arrange

            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns(string.Empty);
            _mockAccount.Setup(a => a.GetUserAsync(It.IsAny<string>())).ReturnsAsync(new UserProfileDTO());

            // Act
            var result = await _accountController.GetUserProfile();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetUserProfile_UserNotFound_ShouldReturnNotFound()
        {
            // Arrange

            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");
            _mockAccount.Setup(a => a.GetUserAsync(It.IsAny<string>())).ReturnsAsync((UserProfileDTO?)null);

            // Act
            var result = await _accountController.GetUserProfile();

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetUserProfile_Ok_ShouldReturOkResult()
        {
            // Arrange
            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");
            _mockAccount.Setup(a => a.GetUserAsync(It.IsAny<string>())).ReturnsAsync(new UserProfileDTO());

            // Act
            var result = await _accountController.GetUserProfile();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetUserProfile_CheckObjectResult_ShouldReturOkResult()
        {
            // Arrange
            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");
            _mockAccount.Setup(a => a.GetUserAsync(It.IsAny<string>())).ReturnsAsync(new UserProfileDTO());

            // Act
            var result = await _accountController.GetUserProfile();

            // Assert
            var objectResult = Assert.IsType<ActionResult<UserProfileDTO>>(result);
        }

        [Fact]
        public async Task GetSelectedBooks_UserIdNotCorrect_ShouldReturBadRequest()
        {
            // Arrange
            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("");
            _mockAccount.Setup(a => a.GetUserSelectedBooksAsync(It.IsAny<string>())).ReturnsAsync(new List<UserSelectedBookDTO>());

            // Act
            var result = await _accountController.GetSelectedBooks();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetSelectedBooks_ObjectIsOk_ShouldReturOkResult()
        {
            // Arrange
            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");
            _mockAccount.Setup(a => a.GetUserSelectedBooksAsync(It.IsAny<string>())).ReturnsAsync(new List<UserSelectedBookDTO>());

            // Act
            var result = await _accountController.GetSelectedBooks();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetSelectedBooks_ObjectIsList_ShouldReturOkResult()
        {
            // Arrange
            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");
            _mockAccount.Setup(a => a.GetUserSelectedBooksAsync(It.IsAny<string>())).ReturnsAsync(new List<UserSelectedBookDTO>());

            // Act
            var result = await _accountController.GetSelectedBooks();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var objectResult = Assert.IsType<ActionResult<ICollection<UserSelectedBookDTO>>>(result);
        }

        [Fact]
        public async Task GetUserComments_UserIdNotCorrect_ShouldReturBadRequest()
        {
            int page = 1;
            CommentType commentType = It.IsAny<CommentType>();
            string searchComment = "";
            string userId = "admin";
            // Arrange
            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns(string.Empty);
            _mockAccount.Setup(a => a.GetUserCommentsAsync(
                commentType, page, searchComment)).ReturnsAsync(new List<CommentProfileDTO>());

            _mockAccount.Setup(a => a.GetCommentsPagesCount(
                commentType, page, searchComment, userId)).ReturnsAsync(1);

            // Act
            var result = await _accountController.GetUserComments(commentType, page, searchComment);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetUserComments_PageNotCorrect_ShouldReturBadRequest()
        {
            int page = 0;
            CommentType commentType = It.IsAny<CommentType>();
            string searchComment = "";
            string userId = "admin";
            // Arrange
            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");
            _mockAccount.Setup(a => a.GetUserCommentsAsync(
                commentType, page, searchComment)).ReturnsAsync(new List<CommentProfileDTO>());

            _mockAccount.Setup(a => a.GetCommentsPagesCount(
                commentType, page, searchComment, userId)).ReturnsAsync(1);

            // Act
            var result = await _accountController.GetUserComments(commentType, page, searchComment);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetUserComments_CommentTypeNotCorrect_ShouldReturBadRequest()
        {
            int page = 1;
            CommentType commentType = (CommentType)999;
            string searchComment = "";
            string userId = "admin";
            // Arrange
            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");
            _mockAccount.Setup(a => a.GetUserCommentsAsync(
                commentType, page, searchComment)).ReturnsAsync(new List<CommentProfileDTO>());

            _mockAccount.Setup(a => a.GetCommentsPagesCount(
                commentType, page, searchComment, userId)).ReturnsAsync(1);

            // Act
            var result = await _accountController.GetUserComments(commentType, page, searchComment);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetUserComments_FirstPage_ShouldReturOkResult()
        {
            int page = 1;
            CommentType commentType = (CommentType)999;
            string searchComment = "";
            string userId = "admin";
            // Arrange
            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");
            _mockAccount.Setup(a => a.GetUserCommentsAsync(
                commentType, page, searchComment)).ReturnsAsync(new List<CommentProfileDTO>());

            _mockAccount.Setup(a => a.GetCommentsPagesCount(
                commentType, page, searchComment, userId)).ReturnsAsync(1);

            // Act
            var result = await _accountController.GetUserComments(commentType, page, searchComment);

            // Assert
            var objectResult = Assert.IsType<ActionResult<CommentPageDTO>>(result);
            Assert.True(objectResult.Value.PagesCount > 0);
        }

        [Fact]
        public async Task GetUserComments_NullCountPage_ShouldReturOkResult()
        {
            int page = 2;
            CommentType commentType = It.IsAny<CommentType>();
            string searchComment = "";
            string userId = "admin";
            // Arrange
            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");
            _mockAccount.Setup(a => a.GetUserCommentsAsync(
                commentType, page, searchComment)).ReturnsAsync(new List<CommentProfileDTO>());

            _mockAccount.Setup(a => a.GetCommentsPagesCount(
                commentType, page, searchComment, userId)).ReturnsAsync(0);

            // Act
            var result = await _accountController.GetUserComments(commentType, page, searchComment);

            // Assert
            var objectResult = Assert.IsType<ActionResult<CommentPageDTO>>(result);
            Assert.True(objectResult.Value.PagesCount == 0);
        }

        [Fact]
        public async Task GetUserComments_OkResult_ShouldReturOkResult()
        {
            int page = 1;
            CommentType commentType = It.IsAny<CommentType>();
            string searchComment = "коммент";
            // Arrange
            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");
            _mockAccount.Setup(a => a.GetUserCommentsAsync(
                commentType, page, searchComment)).ReturnsAsync(new List<CommentProfileDTO>());

            // Act
            var result = await _accountController.GetUserComments(commentType, page, searchComment);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetUserComments_OkObjectResult_ShouldReturOkResult()
        {
            int page = 1;
            CommentType commentType = It.IsAny<CommentType>();
            string searchComment = "коммент";
            // Arrange
            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");
            _mockAccount.Setup(a => a.GetUserCommentsAsync(
                commentType, page, searchComment)).ReturnsAsync(new List<CommentProfileDTO>());

            // Act
            var result = await _accountController.GetUserComments(commentType, page, searchComment);

            // Assert
            var objectResult = Assert.IsType<ActionResult<CommentPageDTO>>(result);
        }



        [Fact]
        public async Task GetFriends_UserIdNotCorrect_ShouldReturBadRequest()
        {
            // Arrange
            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns(string.Empty);
            _mockAccount.Setup(a => a.GetUserFriendsAsync(It.IsAny<string>())).ReturnsAsync(new List<FriendDTO>());

            // Act
            var result = await _accountController.GetFriends();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetFriends_OkResult_ShouldReturOkResult()
        {
            // Arrange
            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");
            _mockAccount.Setup(a => a.GetUserFriendsAsync(It.IsAny<string>())).ReturnsAsync(new List<FriendDTO>());

            // Act
            var result = await _accountController.GetFriends();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetFriends_OkObjectResult_ShouldReturOkResult()
        {
            // Arrange
            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");
            _mockAccount.Setup(a => a.GetUserFriendsAsync(It.IsAny<string>())).ReturnsAsync(new List<FriendDTO>());

            // Act
            var result = await _accountController.GetFriends();

            // Assert
            var objectResult = Assert.IsType<ActionResult<ICollection<FriendDTO>>>(result);
        }


        [Fact]
        public async Task GetUserBooks_UserIdNotCorrect_ShouldReturBadRequest()
        {
            // Arrange
            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns(string.Empty);
            _mockAccount.Setup(a => a.GetUserBooksAsync(It.IsAny<string>())).ReturnsAsync(new List<UserBookDTO>());

            // Act
            var result = await _accountController.GetUserBooks();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetUserBooks_OkResult_ShouldReturOkResult()
        {
            // Arrange
            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");
            _mockAccount.Setup(a => a.GetUserBooksAsync(It.IsAny<string>())).ReturnsAsync(new List<UserBookDTO>());

            // Act
            var result = await _accountController.GetUserBooks();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetUserBooks_OkObjectResult_ShouldReturOkResult()
        {
            // Arrange
            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");
            _mockAccount.Setup(a => a.GetUserBooksAsync(It.IsAny<string>())).ReturnsAsync(new List<UserBookDTO>());

            // Act
            var result = await _accountController.GetUserBooks();

            // Assert
            var objectResult = Assert.IsType<ActionResult<ICollection<UserBookDTO>>>(result);
        }

        [Fact]
        public async Task GetUserBooksUpdates_UserIdNotCorrect_ShouldReturBadRequest()
        {
            // Arrange
            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns(string.Empty);
            _mockAccount.Setup(a => a.CheckUserUpdatesAsync(It.IsAny<string>())).ReturnsAsync(new List<UpdateAccountBook>());

            // Act
            var result = await _accountController.GetUserBooksUpdates();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetUserBooksUpdates_OkResult_ShouldReturOkResult()
        {
            // Arrange
            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");
            _mockAccount.Setup(a => a.CheckUserUpdatesAsync(It.IsAny<string>())).ReturnsAsync(new List<UpdateAccountBook>());

            // Act
            var result = await _accountController.GetUserBooksUpdates();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }


        [Fact]
        public async Task GetUserBooksUpdates_OkObjectResult_ShouldReturOkResult()
        {
            // Arrange
            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");
            _mockAccount.Setup(a => a.CheckUserUpdatesAsync(It.IsAny<string>())).ReturnsAsync(new List<UpdateAccountBook>());

            // Act
            var result = await _accountController.GetUserBooksUpdates();

            // Assert
            var objectResult = Assert.IsType<ActionResult<ICollection<UpdateAccountBook>>>(result);
        }
    }
}
