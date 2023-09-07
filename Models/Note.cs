using AuthorVerseServer.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    [Index("Sectionid")]
    public class Note
    {
        [Key]
        public int NoteId { get; set; }
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
        public int Sectionid { get; set; }
        public ChapterSection Section { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateTime NoteCreatedDateTime { get; set; }
        public PublicationPermission Permission { get; set; }
    }
}
