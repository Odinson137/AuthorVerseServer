using AuthorVerseServer.Data;
using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Repository
{
    public class CommentRatingRepository : ICommentRating
    {
        public readonly DataContext _context;
        public CommentRatingRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckRatingExistAsync(string userId, int commentId)
        {
            return await _context.CommentRatings.AnyAsync(rating =>
                rating.UserCommentedId == userId && rating.CommentId == commentId);
        }

        public async Task AddRatingAsync(Rating rating)
        {
            await _context.CommentRatings.AddAsync(rating);
        }

        public async Task DeleteRatingAsync(int commentId, RatingEntityType entityType)
        {
            await _context.CommentRatings.Where(x => x.CommentId == commentId && x.Discriminator == entityType).ExecuteDeleteAsync();
        }

        public async Task<LikeRating> GetRatingAsync(string userId, int commentId)
        {
            return await _context.CommentRatings
                .Where(rating => rating.UserCommentedId == userId && rating.CommentId == commentId)
                .Select(rating => rating.LikeRating)
                .FirstOrDefaultAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task ChangeRatingAsync(int commentId, RatingEntityType entityType, LikeRating rating)
        {
            await _context.CommentRatings
                .Where(rating => rating.CommentId == commentId && rating.Discriminator == entityType)
                .ExecuteUpdateAsync(setter => setter.SetProperty(r => r.LikeRating, rating));
        }


        //public void ChangeCountRating(int commentId, int downCount, int upCount)
        //{
        //    _context.CommentBases
        //        .Where(comment => comment.BaseId == commentId && rating.Discriminator == entityType
        //        .ExecuteUpdateAsync(setter => setter
        //            .SetProperty(x => x.DisLikes, x => x.DisLikes + downCount)
        //            .SetProperty(x => x.Likes, x => x.Likes + upCount));
        //}
    }
}
