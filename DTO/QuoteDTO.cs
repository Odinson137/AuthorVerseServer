namespace AuthorVerseServer.DTO
{
    public class QuoteDTO
    {
        public int QuoteId { get; set; }
        public UserDTO Quoter { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateOnly QuoteCreatedDateTime { get; set; }
    }
}
