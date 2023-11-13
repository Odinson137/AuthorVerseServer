using AuthorVerseServer.Data;
using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Repository
{
    public class AccountRepository : IAccount
    {
        private readonly DataContext _context;
        public AccountRepository(DataContext context)
        {
            _context = context;
        }

        public Task<ICollection<UpdateAccountBook>> CheckUserUpdates(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<UserProfileDTO> GetUserAsync(string userId)
        {
            return await _context.Users.Select(data => new UserProfileDTO
            {
                UserName = data.UserName,
                Description = data.Description,
                LogoUrl = data.LogoUrl,
            }).FirstOrDefaultAsync();
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
