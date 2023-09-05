using AuthorVerseServer.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthorVerseServer.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public string CommentatorId { get; set; }
        [ForeignKey("CommentatorId")]
        public User Commentator { get; set; } = null!;
        public int BookId { get; set; }
        [ForeignKey("BookId")]
        public Book Book { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateTime CommentCreatedDateTime { get; set; }
        public PublicationPermission Permission { get; set; }
    }
}
