namespace AuthorVerseForum.DTO
{
    public class MessageSendDTO
    {
        public required int BookId { get; set; }
        public required string Text { get; set; }
        public DateTime SendDate { get; set; } = DateTime.Now;
    }

    public class MessageDTO
    {
        public required int BookId { get; set; }
        public required string Text { get; set; }
        public required DateOnly SendDate { get; set; }
        public required int Likes { get; set; }
        public required int Dislikes { get; set; }
        public ICollection<MessageDTO> Replies { get; set; } = new List<MessageDTO>();
    }
}
