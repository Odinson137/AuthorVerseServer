using AuthorVerseServer.Models;

namespace AuthorVerseServer.Interfaces
{
    public interface ICommentRating
    {
        Task<Data.Enums.LikeRating> GetRatingAsync(string userId, int commentId);
        Task<bool> CheckRatingExistAsync(string userId, int commentId);
        Task AddRatingAsync(CommentRating rating);
        Task DeleteRatingAsync(int commentId);
        Task ChangeRatingAsync(int commentId, Data.Enums.LikeRating rating);
        void ChangeCountRating(int commentId, int downCount, int upCount);
        Task SaveAsync();
    }
}
