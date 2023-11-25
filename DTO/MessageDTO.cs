namespace AuthorVerseServer.DTO
{
    public class MessageDTO
    {
        public string Message { get; set; } = null!;
        public MessageDTO() {}
        public MessageDTO(string error)
        {
            Message = error;
        }
    }
}
