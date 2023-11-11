using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class CommentRating
    {
        [Key]
        public int CommentRatingId { get; set; }
        public int CommentId { get; set; }
        public string UserCommentedId { get; set; }
        public int Likes { get; set; }
        public int DisLikes { get; set; }

    }
}
