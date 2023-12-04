using AuthorVerseServer.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class CommentBase
    {
        public int CommentId { get; set; }
        public required string CommentatorId { get; set; }
        public User Commentator { get; set; } = null!;
        public int Likes { get; set; } = 0;
        public int DisLikes { get; set; } = 0;
        public DateTime CommentCreatedDateTime { get; set; } = DateTime.Now;
        public PublicationPermission Permission { get; set; } = PublicationPermission.PendingApproval;
    }
}
