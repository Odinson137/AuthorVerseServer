using AuthorVerseServer.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class CommentBase
    {
        [Key]
        public int BaseId { get; set; }
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
        [MaxLength(1000)]
        public string Text { get; set; } = null!;
        public int Likes { get; set; } = 0;
        public int DisLikes { get; set; } = 0;
        public ICollection<Rating> CommentRatings { get; set; } = new List<Rating>();
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public PublicationPermission Permission { get; set; } = PublicationPermission.Approved;
    }
}
