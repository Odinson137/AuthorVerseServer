using AuthorVerseServer.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class CommentBase
    {
        [Key]
        public int BaseId { get; set; }

        [MaxLength(1000)]
        public string Text { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
        //public int Likes { get; set; } = 0;
        //public int DisLikes { get; set; } = 0;
        public ICollection<Rating> CommentRatings { get; set; } = new List<Rating>();
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string Discriminator { get; set; } = null!;
        public PublicationPermission Permission { get; set; } = PublicationPermission.Approved;
    }
}
