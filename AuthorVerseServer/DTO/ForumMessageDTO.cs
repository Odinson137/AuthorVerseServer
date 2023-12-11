using AuthorVerseServer.Models;

namespace AuthorVerseServer.DTO
{
    public class ForumMessageDTO
    {
        public ForumMessageDTO(string? name, string? lastName, string userName)
        {
            ViewName = string.IsNullOrEmpty(name) ? userName : $"{name} {lastName}";
        }
        public required int MessageId { get; set; }
        public required int? ParrentMessageId { get; set; }
        public string ViewName { get; private set; }
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
