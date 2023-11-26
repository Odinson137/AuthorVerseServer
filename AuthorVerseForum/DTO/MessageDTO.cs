namespace AuthorVerseForum.DTO
{
    public class MessageDTO
    {
        public int BookId { get; set; }
        public required UserDTO User { get; set; }
        public DateOnly SendDate { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public ICollection<MessageDTO> Replies { get; set; } = new List<MessageDTO>();
    }
}
