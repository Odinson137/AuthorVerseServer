using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Models
{
    [Index("GenreId"), Index("BookId")]
    public class BookTag
    {
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
