using AuthorVerseServer.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class Rating
    {
        [Key]
        public int CommentRatingId { get; set; }
        public int CommentId { get; set; }
        public required string UserCommentedId { get; set; }
        public required LikeRating LikeRating { get; set; }
        public required RatingEntityType Discriminator { get; set; }
    }
}
