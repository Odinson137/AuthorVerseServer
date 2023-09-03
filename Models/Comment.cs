using AuthorVerseServer.Enums;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public User Commentator { get; set; } = null!;
        public Book Book { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateTime CommentCreatedDateTime { get; set; }
        public PublicationPermission Permission { get; set; }
    }
}
