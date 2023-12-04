namespace AuthorVerseForum.DTO
{
    public class MessageSendDTO
    {
        public required int MessageId { get; set; }
        public required int BookId { get; set; }
        public AnswerDTO? Answer { get; set; }
        public required string Text { get; set; }
        public DateTime SendDate { get; set; } = DateTime.Now;
    }

    public class AnswerDTO
    {
        public required int MessageId { get; set; }
        public required string ViewName { get; set; }
        public required string Text { get; set; }
    }

    public class MessageDTO
    {
        public required int BookId { get; set; }
        public required string Text { get; set; }
        public required DateOnly SendDate { get; set; }
        //public required int Likes { get; set; }
        //public required int Dislikes { get; set; }
    }

    public class SendForumMessageDTO
    {
        public int? AnswerId { get; set; }
        public required string UserId { get; set; }
        public required int BookId { get; set; }
        public required string Text { get; set; }
    }
}
