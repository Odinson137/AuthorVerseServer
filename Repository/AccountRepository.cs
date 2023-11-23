using AuthorVerseServer.Data;
using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using MailKit.Search;
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

        public async Task<ICollection<UpdateAccountBook>> CheckUserUpdatesAsync(string userId)
        {
            var books = _context.UserSelectedBooks
                .AsNoTracking()
                .Where(book => book.UserId == userId)
                .Where(book => book.LastBookChapterNumber != book.Book.BookChapters.Max(chapter => chapter.BookChapterNumber))
                .Select(book => new UpdateAccountBook
                {
                    BookId = book.BookId,
                    BookTitle = book.Book.Title,
                    ChapterNumber = book.LastBookChapterNumber,
                    BookCoverUrl = book.Book.BookCover,
                });

            return await books.ToListAsync();
        }

        public async Task<UserProfileDTO> GetUserAsync(string userId)
        {
            return await _context.Users.Where(data => data.Id == userId).Select(data => new UserProfileDTO
            {
                UserName = data.UserName,
                Description = data.Description,
                LogoUrl = data.LogoUrl,
            }).FirstOrDefaultAsync();
        }

        private async Task<ICollection<CommentProfileDTO>> GetQueryUserCommentsAsync(CommentType commentType, string searchComment, string userId)
        {
            User user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (commentType == CommentType.All)
            {

            }//Протестировать CommentProfileDTO - в это превратить

            var request = await _context.Comments.Select(x => new CommentProfileDTO
            {

            }).Union(_context.Notes.Select(x => new CommentProfileDTO
            {

            })).ToListAsync();
            return request;

            /*var query = _context.Books
                .Where(book => book.Permission == Data.Enums.PublicationPermission.Approved);

            if (tagId != 0)
            {
                query = query.Where(book => book.Tags.Any(tag => tag.TagId == tagId));
            }

            if (genreId != 0)
            {
                query = query.Where(book => book.Genres.Any(genre => genre.GenreId == genreId));
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(book => book.NormalizedTitle.Contains(searchText));
            }
            */
        }

        public async Task<ICollection<CommentProfileDTO>> GetCommentsPagesCount(CommentType commentType, int page, string searchComment, string userId)
        {//Пользователя
            ICollection<CommentProfileDTO> query = await GetQueryUserCommentsAsync(commentType, searchComment, userId);
            return query;

        }

        public Task<ICollection<CommentProfileDTO>> GetUserCommentsAsync(CommentType commentType, int page, string searchComment)
        {
            //var query = _context.Comments.Where()
            return null;
        }

        public Task<ICollection<UserBookDTO>> GetUserBooksAsync(string userId)
        {
            throw new NotImplementedException();
        }


        public Task<ICollection<FriendDTO>> GetUserFriendsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<UserSelectedBookDTO>> GetUserSelectedBooksAsync(string userId)
        {
            return await _context.UserSelectedBooks
                .Where(data => data.UserId == userId)
                .Select(data=> new UserSelectedBookDTO
                {
                    BookId = data.BookId,
                    Title = data.Book.Title,
                    ImageUrl = data.Book.BookCover,
                    BookState = data.BookState,
                    PublicationData = DateOnly.FromDateTime(data.Book.PublicationData.Date),
                    LastReadingChapter = data.LastBookChapterNumber,
                    LastBookChapter = data.Book.BookChapters.Max(x=> x.BookChapterNumber),
                }).ToListAsync();
        }

        Task<int> IAccount.GetCommentsPagesCount(CommentType commentType, int page, string searchComment, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
