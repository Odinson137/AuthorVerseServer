namespace AuthorVerseServer.DTO
{
    public class QuoteDTO
    {
        public int QuoteId { get; set; }
        public UserDTO User { get; set; } = null!;
        public string Text { get; set; } = null!;
        public int LikeCount { get; set; }
        public int DisLikesCount { get; set; }
        public DateOnly QuoteCreatedDateTime { get; set; }
    }
}
