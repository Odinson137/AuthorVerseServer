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
        public string? Description { get; set; }
        public string? CharacterImageUrl { get; set; }
    }
}
