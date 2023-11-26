using AuthorVerseServer.Models;

namespace AuthorVerseServer.DTO
{
    public class BookChapterDTO
    {
        public int BookChapterId { get; set; }
        public string? Description { get; set; }
        public int BookId { get; set; }
        public DateTime PublicationData { get; set; }
    }
}
