using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;

namespace AuthorVerseServer.Models
{
    [Index("BookId"), Index("BookChapterId")]
    public class Character
    {
        [Key]
        public int CharacterId { get; set; }
        public int BookId { get; set; }
        [ForeignKey("BookId")]
        public Book Book { get; set; } = null!;
        public int BookChapterId { get; set; }
        //[ForeignKey("BookChapterId")]
        //public BookChapter BookChapter { get; set; } = null!;
        public string? Description { get; set; }
        public Image? CharacterImage { get; set; }
    }
}
