using System.ComponentModel.DataAnnotations;

namespace AuthorVerseForum.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        public required int BookId { get; set; } // & Название чата
        public Message? ParrentMessage { get; set; }
        public int ParrentMessageId { get; set; }
        public List<Message> AnswerMessages { get; set; } = new List<Message>();
        public required string Text { get; set; }
        public DateTime SendTime { get; set; } = DateTime.Now; 
    }
}
