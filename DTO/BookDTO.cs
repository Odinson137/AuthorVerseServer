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
        public ICollection<GenreDTO>? Genres { get; set; } = new List<GenreDTO>();
        public ICollection<TagDTO>? Tags { get; set; } = new List<TagDTO>();
        public AgeRating AgeRating { get; set; }
        public string? BookCoverUrl { get; set; }
    }

    public class CreateBookDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string AuthorId { get; set; } = null!;
        public ICollection<int>? GenresId { get; set; }
        public AgeRating AgeRating { get; set; }
        public string? BookCoverUrl { get; set; }
        public string? BookPanoramUrl { get; set; }
    }

    public class PopularBook
    {
        public int BookId { get; set; }
        public string? BookCoverUrl { get; set; }
    }

    //public class BookPage
    //{
    //    public int BookId { get; set; }
    //    public Image BookCover { get; set; } = null!;
    //}
}
