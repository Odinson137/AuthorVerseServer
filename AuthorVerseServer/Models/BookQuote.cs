
namespace AuthorVerseServer.Models
{
    public class BookQuote : CommentBase
    {
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
    }
}
