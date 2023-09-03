namespace AuthorVerseServer.Models
{
    public class BookChapter
    {
        public int BookChapterId { get; set; }
        public ICollection<ChapterSection> ChapterSections { get; set; } = null!;
        public DateTime PublicationData { get; set; }
    }
}
