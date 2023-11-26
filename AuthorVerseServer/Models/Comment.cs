using AuthorVerseServer.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public string CommentatorId { get; set; } = null!;
        public User Commentator { get; set; } = null!;
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
        [MaxLength(1000)]
        [MinLength(50)]
        public string Text { get; set; } = null!;
        [Range(1, 5)]
        public int ReaderRating { get; set; }
        public int Likes { get; set; } = 0;
        public int DisLikes { get; set; } = 0;
        public DateTime CommentCreatedDateTime { get; } = DateTime.Now;
        public ICollection<CommentRating> CommentRatings { get; set; } = new List<CommentRating>();
        public PublicationPermission Permission { get; set; } = PublicationPermission.PendingApproval;
    }
}
