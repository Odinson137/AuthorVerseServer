using Org.BouncyCastle.Crypto.Utilities;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class ForumMessage
    {
        [Key]
        public int MessageId { get; set; }
        public Book Book { get; set; } = null!;
        public required int BookId { get; set; } // & Название чата
        public User User { get; set; } = null!;
        public required string UserId { get; set; }
        public int? ParrentMessageId { get; set; }
        public ForumMessage? ParrentMessage { get; set; }
        public ICollection<ForumMessage> AnswerMessages { get; set; } = new List<ForumMessage>();
        public required string Text { get; set; }
        public DateTime SendTime { get; set; } = DateTime.Now;
    }
}
