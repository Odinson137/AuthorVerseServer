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

        public async Task<int> ChechExistBookAsync(int bookId)
        {
            return await _context.Books.Where(x => x.BookId == bookId).Select(x => x.BookId).FirstOrDefaultAsync();
        }

        public async Task AddComment(Comment newComment)
        {
            await _context.Comments.AddAsync(newComment);
        }

        public async Task DeleteComment(int commentId)
        {
            await _context.Comments.Where(c => c.BaseId == commentId).ExecuteDeleteAsync();
        }

        public async Task<Comment?> GetCommentAsync(int commentId)
        {
            return await _context.Comments.FirstOrDefaultAsync(x=> x.BaseId == commentId);
        }

        public async Task<int> Save()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<ICollection<CommentDTO>> GetCommentsByBookAsync(int bookId, int page, string? userId)
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

            return await comments.ToListAsync();
        }

        public async Task<bool> CheckExistCommentAsync(int commentId, string userId)
        {
            return await _context.Comments.Where(x => x.BaseId == commentId & x.UserId == userId).AnyAsync();
        }
    }
}
