using AuthorVerseServer.Enums;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class Note
    {
        [Key]
        public int NoteId { get; set; }
        public User User { get; set; } = null!;
        public ChapterSection Section { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateTime NoteCreatedDateTime { get; set; }
        public PublicationPermission Permission { get; set; }
    }
}
