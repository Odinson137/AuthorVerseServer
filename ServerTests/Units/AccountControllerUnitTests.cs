using AuthorVerseServer.Controllers;
using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Interfaces.ServiceInterfaces;
using AuthorVerseServer.Models;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private readonly Mock<CreateJWTtokenService> _mockJWTTokenService;
        private readonly Mock<ILoadImage> _mockLoadImage;

        public AccountControllerUnitTests()
        {
            _mockAccount = new Mock<IAccount>();
            _mockLoadImage = new Mock<ILoadImage>();
            _mockUserManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _mockJWTTokenService = new Mock<CreateJWTtokenService>();

            _accountController = new AccountController(
                _mockAccount.Object, _mockJWTTokenService.Object, _mockUserManager.Object, _mockLoadImage.Object);
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
            _mockAccount.Setup(a => a.GetUserSelectedBooksAsync(It.IsAny<string>())).ReturnsAsync(new List<SelectedUserBookDTO>());

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
            _mockAccount.Setup(a => a.GetUserSelectedBooksAsync(It.IsAny<string>())).ReturnsAsync(new List<SelectedUserBookDTO>());

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
            _mockAccount.Setup(a => a.GetUserSelectedBooksAsync(It.IsAny<string>())).ReturnsAsync(new List<SelectedUserBookDTO>());

            // Act
            var result = await _accountController.GetSelectedBooks();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var objectResult = Assert.IsType<ActionResult<ICollection<SelectedUserBookDTO>>>(result);
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
                commentType, page, searchComment, "admin")).ReturnsAsync(new List<CommentProfileDTO>());

            _mockAccount.Setup(a => a.GetCommentsPagesCount(
                commentType, searchComment, userId)).ReturnsAsync(1);

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
                commentType, page, searchComment, "admin")).ReturnsAsync(new List<CommentProfileDTO>());

            _mockAccount.Setup(a => a.GetCommentsPagesCount(
                commentType, searchComment, userId)).ReturnsAsync(1);

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
                commentType, page, searchComment, "admin")).ReturnsAsync(new List<CommentProfileDTO>());

            _mockAccount.Setup(a => a.GetCommentsPagesCount(
                commentType, searchComment, userId)).ReturnsAsync(1);

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
                commentType, page, searchComment, "admin")).ReturnsAsync(new List<CommentProfileDTO>());

            _mockAccount.Setup(a => a.GetCommentsPagesCount(
                commentType, searchComment, userId)).ReturnsAsync(1);

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
                commentType, page, searchComment, "admin")).ReturnsAsync(new List<CommentProfileDTO>());

            _mockAccount.Setup(a => a.GetCommentsPagesCount(
                commentType,  searchComment, userId)).ReturnsAsync(0);

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
                commentType, page, searchComment, "admin")).ReturnsAsync(new List<CommentProfileDTO>());

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
                commentType, page, searchComment, "admin")).ReturnsAsync(new List<CommentProfileDTO>());

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

        [Fact]
        public async Task ChangeUserProfiles_ErrorToken_ShouldReturBadRequest()
        {
            // Arrange
            var profileUser = new EditProfileDTO();

            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("");

            // Act
            var result = await _accountController.ChangeUserProfile(profileUser) as ObjectResult;

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public async Task ChangeUserProfiles_UserNotFound_ShouldReturNotFound()
        {
            // Arrange
            var profileUser = new EditProfileDTO();
            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");
            _mockUserManager.Setup(a => a.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

            // Act
            var result = await _accountController.ChangeUserProfile(profileUser) as ObjectResult;

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task ChangeUserProfiles_PasswordDoesNotmatch_ShouldReturBadRequest()
        {
            // Arrange
            var profileUser = new EditProfileDTO();
            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");
            _mockUserManager.Setup(a => a.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());

            var failedIdentityResult = new IdentityResult();
            var type = failedIdentityResult.GetType();
            var succeededProperty = type.GetProperty("Succeeded");
            succeededProperty.SetValue(failedIdentityResult, false, null);

            _mockUserManager.Setup(a => a.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(failedIdentityResult);

            // Act
            var result = await _accountController.ChangeUserProfile(profileUser) as ObjectResult;

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ChangeUserProfiles_PasswordChange_ShouldReturBadRequest()
        {
            // Arrange
            var profileUser = new EditProfileDTO();
            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");
            _mockUserManager.Setup(a => a.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());

            var failedIdentityResult = new IdentityResult();
            var type = failedIdentityResult.GetType();
            var succeededProperty = type.GetProperty("Succeeded");
            succeededProperty.SetValue(failedIdentityResult, true, null);

            _mockUserManager.Setup(a => a.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(failedIdentityResult);

            _mockAccount.Setup(a => a.SaveAsync());

            // Act
            var result = await _accountController.ChangeUserProfile(profileUser);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ChangeUserProfiles_ChangeImage_ShouldReturBadRequest()
        {
            // Arrange
            var formFileMock = new Mock<IFormFile>();
            var profileUser = new EditProfileDTO
            {
                Logo = formFileMock.Object
            };

            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");
            _mockUserManager.Setup(a => a.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());

            var failedIdentityResult = new IdentityResult();
            var type = failedIdentityResult.GetType();
            var succeededProperty = type.GetProperty("Succeeded");
            succeededProperty.SetValue(failedIdentityResult, true, null);

            _mockUserManager.Setup(a => a.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(failedIdentityResult);

            _mockLoadImage.Setup(a => a.GetUniqueName(It.IsAny<IFormFile>())).Returns("uniqueName");
            _mockLoadImage.Setup(a => a.CreateImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), "Images"));

            _mockAccount.Setup(a => a.SaveAsync());

            // Act
            var result = await _accountController.ChangeUserProfile(profileUser);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ChangeUserProfiles_ClearImage_ShouldReturBadRequest()
        {
            // Arrange
            var profileUser = new EditProfileDTO();

            _mockJWTTokenService.Setup(cl => cl.GetIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("admin");
            _mockUserManager.Setup(a => a.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User() { LogoUrl = "need_to_clear"});

            var failedIdentityResult = new IdentityResult();
            var type = failedIdentityResult.GetType();
            var succeededProperty = type.GetProperty("Succeeded");
            succeededProperty.SetValue(failedIdentityResult, true, null);

            _mockUserManager.Setup(a => a.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(failedIdentityResult);

            _mockLoadImage.Setup(a => a.GetUniqueName(It.IsAny<IFormFile>())).Returns("uniqueName");
            _mockLoadImage.Setup(a => a.CreateImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), "Images"));

            _mockAccount.Setup(a => a.SaveAsync());

            // Act
            var result = await _accountController.ChangeUserProfile(profileUser);

            // Assert
            Assert.IsType<OkResult>(result);
        }
    }
}
