using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.DTO;
using Microsoft.EntityFrameworkCore;
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
        [MaxLength(400)]
        [MinLength(50)]
        public string Text { get; set; } = null!;
        public DateTime CommentCreatedDateTime { get; } = DateTime.Now;
        public ICollection<CommentRating> commentRatings { get; set; } = new List<CommentRating>();
        public PublicationPermission Permission { get; set; } = PublicationPermission.PendingApproval;
    }
}
