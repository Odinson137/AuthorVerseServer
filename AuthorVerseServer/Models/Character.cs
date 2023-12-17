using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class Character
    {
        [Key]
        public int CharacterId { get; set; }
        [MaxLength(200)]
        public required string Name { get; set; }
        [MaxLength(200)]
        public string NormalizedName { get; set; } = null!;
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
        public ICollection<BookChapter> BookChapters { get; set; } = new List<BookChapter>();
        //public int BookChapterId { get; set; }
        //public BookChapter BookChapter { get; set; } = null!;
        [MaxLength(200)]
        public string? Description { get; set; }
        public string? CharacterImageUrl { get; set; }
    }
}
