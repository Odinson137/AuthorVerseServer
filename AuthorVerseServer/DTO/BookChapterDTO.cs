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
        public required int BookChapterId { get; set; }
        public required int Number { get; set; }
        public required string? Title { get; set; }
        public required string? Place { get; set; }
        public required int CharacterCount { get; set; }  
    }

    public class DetaildAuthorChapterDTO
    {
        public string? Description { get; set; }
        public ICollection<CharacterDTO>? Characters { get; set; }
    }

    public class NotifyChapter
    {
        public required int BookId { get; set; }
        public required string BookTitle { get; set; }
        public required int ChapterNumber { get; set; }
        public string? ChapterTitle { get; set; }
        public string? Url { get; set; }
    }

    public class ChapterInfo
    {
        public required int BookId { get; set; }
        public required int ChapterNumber { get; set; }
    }
}
