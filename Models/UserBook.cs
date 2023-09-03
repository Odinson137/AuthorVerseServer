using AuthorVerseServer.Enums;

namespace AuthorVerseServer.Models
{
    public class UserBook
    {
        public int UserBookId { get; set; }
        public Book Book { get; set; } = null!;
        public BookState BookState { get; set; }
        public BookChapter? LastChapter { get; set; }
    }
}
