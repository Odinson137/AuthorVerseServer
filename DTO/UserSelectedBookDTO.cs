using AuthorVerseServer.Data.Enums;

namespace AuthorVerseServer.DTO
{
    public class UserSelectedBookDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public BookState BookState { get; set; }
        public DateOnly PublicationData { get; set; }
        public int LastReadingChapter { get; set; }
        public int LastBookChapter { get; set; }
    }
}
