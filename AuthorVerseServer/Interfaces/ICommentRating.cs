using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AuthorVerseServer.Interfaces
{
    public interface ICommentRating
    {
        Task<Data.Enums.LikeRating> GetRatingAsync(string userId, int commentId);
        Task<bool> CheckRatingExistAsync(string userId, int commentId);
        ValueTask AddRatingAsync(Rating rating);
        Task DeleteRatingAsync(int commentId, RatingEntityType entityType);
        Task ChangeRatingAsync(int commentId, RatingEntityType entityType, LikeRating rating);
        //void ChangeCountRating(int commentId, Data.Enums.RatingEntityType entityType, int downCount, int upCount);
        Task SaveAsync();
    }
}
