using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.DTO;

namespace AuthorVerseServer.Interfaces
{
    public interface IAccount
    {
        Task<UserProfileDTO> GetUserAsync(string userId);
        Task<ICollection<UserSelectedBookDTO>> GetUserSelectedBooksAsync(string userId); // то что читает пользователь
        Task<ICollection<CommentProfileDTO>> GetUserCommentsAsync(CommentType commentType, int page, string searchComment);
        Task<ICollection<FriendDTO>> GetUserFriendsAsync(string userId);
        Task<ICollection<UserBookDTO>> GetUserBooksAsync(string userId); // то что сам пишет
    }
}
