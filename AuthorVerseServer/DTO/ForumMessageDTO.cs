﻿namespace AuthorVerseServer.DTO
{
    public class ForumMessageDTO
    {
        public required int MessageId { get; set; }
        public required string ViewName { get; set; }
        public required string Text { get; set; }
        public required DateTime SendTime { get; set; }
        //public ICollection<ForumMessageDTO> Messages { get; set; } = new List<ForumMessageDTO>();
    }

    public class SendForumMessageDTO
    {
        public int AnswerId { get; set; }
        public required string UserId { get; set; }
        public int BookId { get; set; }
        public required string Text { get; set; }
    }
}
