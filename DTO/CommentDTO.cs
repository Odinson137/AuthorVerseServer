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

    public class CreateCommentDTO
    {
        public string UserId { get; set; }
        public int BookId { get; set; }
        public string Text { get; set; }
    }
}
