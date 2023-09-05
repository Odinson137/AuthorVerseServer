using AuthorVerseServer.Enums;
using AuthorVerseServer.Models;

namespace AuthorVerseServer.DTO
{
    public class CommentDTO
    {
        public int CommentId { get; set; }
        public UserDTO Commentator { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateTime CommentCreatedDateTime { get; set; }
    }
}
