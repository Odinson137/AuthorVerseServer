using AuthorVerseServer.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthorVerseServer.Models
{
    [Index("BookId"), Index("CommentatorId")]
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public string CommentatorId { get; set; }
        [ForeignKey("CommentatorId")]
        public User Commentator { get; set; } = null!;
        public int BookId { get; set; }
        public string Text { get; set; } = null!;
        public int Likes { get; set; }
        public int DisLikes { get; set; }
        public int Rating { get; set; }
        public DateTime CommentCreatedDateTime { get; set; }
        public PublicationPermission Permission { get; set; }
    }
}
