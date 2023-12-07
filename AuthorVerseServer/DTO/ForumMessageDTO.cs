using AuthorVerseServer.Models;

namespace AuthorVerseServer.DTO
{
    public class ForumMessageDTO
    {
        public required int MessageId { get; set; }
        public required int? ParrentMessageId { get; set; }
        public required string ViewName { get; set; }
        public required string Text { get; set; }
        public required DateTime SendTime { get; set; }
    }

    public class SendForumMessageDTO
    {
        public int? AnswerId { get; set; }
        public required string UserId { get; set; }
        public required int BookId { get; set; }
        public required string Text { get; set; }
    }

    public class ChangeTextDTO
    {
        public required int MessageId { get; set; }
        public required string NewText { get; set; }
    }
    public class DeleteMessageDTO
    {
        public required int MessageId { get; set; }
        public required string UserId { get; set; }
    }

}
