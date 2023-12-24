using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.DTO;

namespace AuthorVerseServer.Interfaces
{
    public interface IAccount
    {
        Task SaveAsync();
        Task<UserProfileDTO> GetUserAsync(string userId);
        Task<List<SelectedUserBookDTO>> GetUserSelectedBooksAsync(string userId); // то что читает пользователь
        Task<CommentPageDTO> GetUserCommentsAsync(CommentType commentType, int page, string searchComment, string userId);
        Task<List<FriendDTO>> GetUserFriendsAsync(string userId);
        Task<List<UserBookDTO>> GetUserBooksAsync(string userId); // то что сам пишет

        Task<List<UpdateAccountBook>> CheckUserUpdatesAsync(string userId); // должен выводить последние вышедшие главы, если юзер их ещё не прочитал
    }
}
