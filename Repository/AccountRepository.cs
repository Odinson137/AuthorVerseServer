using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;

namespace AuthorVerseServer.Repository
{
    public class AccountRepository : IAccount
    {
        public Task<ICollection<UpdateAccountBook>> CheckUserUpdates(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetCommentsPagesCount(CommentType commentType, int page, string searchComment)
        {
            throw new NotImplementedException();
        }

        public Task<UserProfileDTO> GetUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<UserBookDTO>> GetUserBooksAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<CommentProfileDTO>> GetUserCommentsAsync(CommentType commentType, int page, string searchComment)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<FriendDTO>> GetUserFriendsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<UserSelectedBookDTO>> GetUserSelectedBooksAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
