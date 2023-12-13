namespace AuthorVerseServer.DTO
{
    public class PageBookChaptersDTO
    {
        public required int LastReadingNumber { get; set; }
        public required ICollection<BookChapterDTO>? BookChapters { get; set; }
    }
    public class BookChapterDTO
    {
        public required int BookChapterId { get; set; }
        public required int Number { get; set; }
        public string? Title { get; set; }
    }

    public class ShortAuthorChapterDTO
    {
        public int BookChapterId { get; set; }
        public string? Number { get; set; }
        public int Title { get; set; }
        public string? Place { get; set; }
        public int CharacterCount { get; set; }  
    }

    public class DetaildAuthorChapterDTO
    {
        public string? Description { get; set; }
        public ICollection<CharacterDTO>? Characters { get; set; }
    }
}
