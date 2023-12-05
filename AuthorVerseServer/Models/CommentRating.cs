using AuthorVerseServer.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class CommentRating
    {
        [Key]
        public int CommentRatingId { get; set; }
        public int BaseId { get; set; }
        public string UserCommentedId { get; set; }
        public LikeRating Rating { get; set; }
    }
}
