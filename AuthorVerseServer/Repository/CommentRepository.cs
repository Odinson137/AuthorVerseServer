using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Repository
{
    public class CommentRepository : IComment
    {
        private readonly DataContext _context;
        public CommentRepository(DataContext context)
        {
            _context = context;
        }

        public Task<int> ChechExistBookAsync(int bookId)
        {
            return _context.Books.Where(x => x.BookId == bookId).Select(x => x.BookId).FirstOrDefaultAsync();
        }

        public Task AddComment(Comment newComment)
        {
            _context.Comments.AddAsync(newComment);
            return Task.CompletedTask;
        }

        public Task DeleteComment(int commentId)
        {
            _context.Comments.Where(c => c.BaseId == commentId).ExecuteDeleteAsync();
            return Task.CompletedTask;
        }

        public Task<Comment?> GetCommentAsync(int commentId)
        {
            return _context.Comments.FirstOrDefaultAsync(x=> x.BaseId == commentId);
        }

        public async Task<int> Save()
        {
            return await _context.SaveChangesAsync();
        }

        public Task<List<CommentDTO>> GetCommentsByBookAsync(int bookId, int page, string? userId)
        {
            var comments = _context.Comments
                .AsNoTracking()
                .OrderByDescending(c => c.BaseId)
                .Where(c => c.BookId == bookId)
                .Skip(page * 5)
                .Take(5)
                .Select(c => new CommentDTO
                {
                    BaseId = c.BaseId,
                    User = new UserViewDTO(c.User.Name, c.User.LastName, c.User.UserName)
                    {
                        Id = c.UserId,
                        LogoUrl = c.User.LogoUrl
                    },
                    Text = c.Text,
                    ReaderRating = c.ReaderRating,
                    Likes = c.CommentRatings.Count(x => x.LikeRating == Data.Enums.LikeRating.Like),
                    DisLikes = c.CommentRatings.Count(x => x.LikeRating == Data.Enums.LikeRating.DisLike),
                    //Likes = c.Likes,
                    //DisLikes = c.DisLikes,
                    IsRated = string.IsNullOrEmpty(userId) ? Data.Enums.LikeRating.NotRated :
                        c.CommentRatings.Where(r => r.UserCommentedId == userId)
                        .Select(x => x.LikeRating).FirstOrDefault(),
                    CreatedDateTime = DateOnly.FromDateTime(c.CreatedDateTime),
                });

            return comments.ToListAsync();
        }

        public Task<bool> CheckExistCommentAsync(int bookId, string userId)
        {
            return _context.Comments.Where(x => x.BookId == bookId & x.UserId == userId).AnyAsync();
        }
    }
}
