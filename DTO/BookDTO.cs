using AuthorVerseServer.Enums;
using AuthorVerseServer.Models;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.DTO
{
    public class BookDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public User Author { get; set; }
        public ICollection<Genre> Genres { get; set; }
        public AgeRating AgeRating { get; set; }
    }
}
