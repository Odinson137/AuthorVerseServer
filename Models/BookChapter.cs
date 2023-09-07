using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    [Index("BookId")]
    public class BookChapter
    {
        [Key]
        public int BookChapterId { get; set; }
        public string? Description { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public ICollection<ChapterSection> ChapterSections { get; set; } = new List<ChapterSection>();
        public DateTime PublicationData { get; set; }
    }
}
