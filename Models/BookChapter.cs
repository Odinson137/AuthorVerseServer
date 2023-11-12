using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class BookChapter
    {
        [Key]
        public int BookChapterId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public ICollection<int> Characters { get; set; } = new List<int>();
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
        public ICollection<Note>? Notes { get; set; }
        public ICollection<ChapterSection> ChapterSections { get; set; } = new List<ChapterSection>();
        public DateTime PublicationData { get; set; } = DateTime.Now;
    }
}
