using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.DTO;
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
        public string CommentatorId { get; set; } = null!;
        public User Commentator { get; set; } = null!;
        public UserDTO CommentatorDTO { get; set; } = null!;
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
        public string Text { get; set; } = null!;
        public int Likes { get; set; } = 0;
        public int DisLikes { get; set; } = 0;
        public DateTime CommentCreatedDateTime { get; } = DateTime.Now;
        public PublicationPermission Permission { get; set; } = PublicationPermission.PendingApproval;
    }
}
