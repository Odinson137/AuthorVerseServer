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

        private async Task<IQueryable<Comment>> GetQueryUserCommentsAsync(CommentType commentType, string searchComment, string userId)
        {
            User user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if(commentType == CommentType.All)
            {
                return (IQueryable<Comment>)user.Comments.Where(x => x.CommentatorId == userId);
            }//Протестировать
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
            return (IQueryable<Comment>)user.Comments.Where(x => x.CommentatorId == userId);
        }

        public async Task<int> GetCommentsPagesCount(CommentType commentType, int page, string searchComment, string userId)
        {//Пользователя
            IQueryable<Comment> query = await GetQueryUserCommentsAsync(commentType, searchComment, userId);
            return await query.CountAsync();

        }

        public Task<ICollection<CommentProfileDTO>> GetUserCommentsAsync(CommentType commentType, int page, string searchComment)
        {
            //var query = _context.Comments.Where()
            return null;
        }

        public async Task<ICollection<UserBookDTO>> GetUserBooksAsync(string userId)
        {
            var books = _context.Books
                .Where(book => book.AuthorId == userId)
                .Select(book => new UserBookDTO
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Description = book.Description,
                    BookCoverUrl = book.BookCover,
                    PublicationData = DateOnly.FromDateTime(book.PublicationData),
                    Earnings = book.Earnings
                });

            return await books.ToListAsync();
        }


        public Task<ICollection<FriendDTO>> GetUserFriendsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<SelectedUserBookDTO>> GetUserSelectedBooksAsync(string userId)
        {
            return await _context.UserSelectedBooks
                .Where(data => data.UserId == userId)
                .Select(data=> new SelectedUserBookDTO
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
    }
}
