namespace AuthorVerseServer.DTO
{
    public class ErrorMessageDTO
    {
        public string Message { get; set; } = null!;
        public ErrorMessageDTO() {}
        public ErrorMessageDTO(string error)
        {
            Message = error;
        }
    }
}
