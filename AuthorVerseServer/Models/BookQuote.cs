
namespace AuthorVerseServer.Models
{
    public class BookQuote : CommentBase
    {
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
    }
}
