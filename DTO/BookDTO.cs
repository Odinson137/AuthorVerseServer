using AuthorVerseServer.Enums;
using AuthorVerseServer.Models;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.DTO
{
    public class BookDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public UserDTO Author { get; set; } = null!;
        public ICollection<GenreDTO>? Genres { get; set; }
        public AgeRating AgeRating { get; set; }
    }
}
