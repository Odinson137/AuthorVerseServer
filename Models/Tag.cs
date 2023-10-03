using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<Book>? Books { get; set; }
    }
}
