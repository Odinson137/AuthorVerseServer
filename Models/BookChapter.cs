using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class BookChapter
    {
        [Key]
        public int BookChapterId { get; set; }
        public ICollection<ChapterSection> ChapterSections { get; set; } = null!;
        public DateTime PublicationData { get; set; }
    }
}
