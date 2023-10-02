using AuthorVerseServer.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    [Index("BookChapterid")]
    public class Note
    {
        [Key]
        public int NoteId { get; set; }
        public string UserId { get; set; } = null!;
        public int BookChapterid { get; set; }
        public string Text { get; set; } = null!;
        public DateTime NoteCreatedDateTime { get; set; }
        public PublicationPermission Permission { get; set; }
    }
}
