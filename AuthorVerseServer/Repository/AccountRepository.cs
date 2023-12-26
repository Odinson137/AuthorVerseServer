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

        public  Task<List<UpdateAccountBook>> CheckUserUpdatesAsync(string userId)
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

            return  books.ToListAsync();
        }

        public  Task<UserProfileDTO> GetUserAsync(string userId)
        {
            return  _context.Users.Where(data => data.Id == userId).Select(data => new UserProfileDTO
            {
                UserName = data.UserName,
                Description = data.Description,
                LogoUrl = data.LogoUrl,
            }).FirstAsync();
        }

        private IQueryable<CommentProfileDTO> GetQueryForChapterComments(string userId)
        {
            return _context.Notes
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedDateTime)
                .Select(x => new CommentProfileDTO
                {
                    BaseId = x.BaseId,
                    Text = x.Text,
                    Likes = x.CommentRatings.Count(x => x.LikeRating == LikeRating.Like),
                    DisLikes = x.CommentRatings.Count(x => x.LikeRating == LikeRating.DisLike),
                    CommentType = CommentType.Chapter,
                    BookTitle = x.BookChapter.Book.Title,
                    ChapterNumber = x.BookChapter.BookChapterNumber,
                    ChapterTitle = x.BookChapter.Title,
                    CreatedDateTime = DateOnly.FromDateTime(x.CreatedDateTime)
                });
        }

        private IQueryable<CommentProfileDTO> GetQueryForBookComments(string userId)
        {
            return _context.Comments
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedDateTime)
                .Select(x => new CommentProfileDTO
                {
                    BaseId = x.BaseId,
                    Text = x.Text,
                    Rating = x.ReaderRating,
                    Likes = x.CommentRatings.Count(x => x.LikeRating == LikeRating.Like),
                    DisLikes = x.CommentRatings.Count(x => x.LikeRating == LikeRating.DisLike),
                    //Likes = x.Likes,
                    //DisLikes = x.DisLikes,
                    CommentType = CommentType.Book,
                    BookTitle = x.Book.Title,
                    CreatedDateTime = DateOnly.FromDateTime(x.CreatedDateTime)
                });
        }

        private IQueryable<CommentProfileDTO> GetQueryForDefaultComments()
        {
            return _context.CommentBases
                .GroupJoin(
                    _context.Notes,
                    commentBase => commentBase.BaseId,
                    note => note.BaseId,
                    (commentBase, noteGroup) => new { commentBase, noteGroup }
                )
                .SelectMany(
                    combined => combined.noteGroup.DefaultIfEmpty(),
                    (combined, note) => new { combined.commentBase, note }
                )
                .GroupJoin(
                    _context.Comments,
                    combined => combined.commentBase.BaseId,
                    comment => comment.BaseId,
                    (combined, commentGroup) => new { combined.commentBase, combined.note, commentGroup }
                )
                .SelectMany(
                    combined => combined.commentGroup.DefaultIfEmpty(),
                    (combined, comment) => new { combined.commentBase, combined.note, comment }
                )
                .OrderByDescending(x => x.commentBase.CreatedDateTime)
                .Select(dto => new CommentProfileDTO
                {
                    BaseId = dto.commentBase.BaseId,
                    Text = dto.commentBase.Text,
                    Rating = dto.comment != null ? dto.comment.ReaderRating : 0,
                    Likes = dto.commentBase.CommentRatings.Count(x => x.LikeRating == LikeRating.Like), 
                    DisLikes = dto.commentBase.CommentRatings.Count(x => x.LikeRating == LikeRating.DisLike),
                    CommentType = dto.note != null ? CommentType.Chapter : CommentType.Book,
                    BookTitle = dto.comment != null ? dto.comment.Book.Title : dto.note.BookChapter.Book.Title,
                    ChapterNumber = dto.note != null ? dto.note.BookChapter.BookChapterNumber : 0,
                    ChapterTitle = dto.note != null ? dto.note.BookChapter.Title : "",
                    CreatedDateTime = DateOnly.FromDateTime(dto.commentBase.CreatedDateTime)
                });
        }

        private IQueryable<CommentProfileDTO> GetQueryUserCommentsAsync(CommentType commentType, string searchComment, string userId)
        {
            IQueryable<CommentProfileDTO> query;
            if (commentType == CommentType.Chapter)
            {
                query = GetQueryForChapterComments(userId);
            }
            else if (commentType == CommentType.Book)
            {
                query = GetQueryForBookComments(userId);
            }
            else
            {
                query = GetQueryForDefaultComments();
            }

            if (!string.IsNullOrEmpty(searchComment))
            {
                query = query.Where(x => x.Text.Contains(searchComment));
            }

            return query;
        }

        public async Task<CommentPageDTO> GetUserCommentsAsync(CommentType commentType, int page, string searchComment, string userId)
        {
            IQueryable<CommentProfileDTO> query = GetQueryUserCommentsAsync(commentType, searchComment, userId);

            int commentsCount = page == 0 ? await query.CountAsync() : 0;

            var commentsFounded = await query
                .Skip(page * 5)
                .Take(5)
                .ToListAsync();

            return new CommentPageDTO
            {
                PagesCount = commentsCount,
                Comments = commentsFounded
            };
        }

        public  Task<List<UserBookDTO>> GetUserBooksAsync(string userId)
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

            return books.ToListAsync();
        }


        public async Task<List<FriendDTO>> GetUserFriendsAsync(string userId)
        {
            var friends = await _context.Friendships
                .Where(x => x.User1Id == userId)
                .Select(user => new FriendDTO
                {
                    Id = user.User2.Id,
                    UserName = user.User2.UserName,
                    Status = user.Status,
                    FriendShipTime = DateOnly.FromDateTime(user.FriendshipStart),
                }).ToListAsync();

            var friendB = await _context.Friendships
                .Where(x => x.User2Id == userId)
                .Where(user => user.Status == FriendshipStatus.Accepted)
                .Select(user => new FriendDTO
                {
                    Id = user.User1.Id,
                    UserName = user.User1.UserName,
                    Status = user.Status,
                    FriendShipTime = DateOnly.FromDateTime(user.FriendshipStart),
                }).ToListAsync();

            var friendC = friends.Union(friendB);

            return friendC.ToList();
        }

        public  Task<List<SelectedUserBookDTO>> GetUserSelectedBooksAsync(string userId)
        {
            return  _context.UserSelectedBooks
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

        public Task<int> SaveAsync()
        {
             return _context.SaveChangesAsync();
        }
    }
}
