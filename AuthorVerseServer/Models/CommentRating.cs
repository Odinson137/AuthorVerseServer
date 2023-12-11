using AuthorVerseServer.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class CommentRating
    {
        [Key]
        public int CommentRatingId { get; set; }
        public int CommentId { get; set; }
        public required string UserCommentedId { get; set; }
        public required LikeRating Rating { get; set; }
    }
}
