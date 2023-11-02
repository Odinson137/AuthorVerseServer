using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.Models;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.DTO
{
    public class BookPageDTO
    {
        public ICollection<BookDTO> Books { get; set; } = new List<BookDTO>();
        public int BooksCount { get; set; }
    }

    public class BookDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public UserDTO Author { get; set; } = null!;
        public ICollection<GenreDTO> Genres { get; set; } = new List<GenreDTO>();
        public ICollection<TagDTO> Tags { get; set; } = new List<TagDTO>();
        public AgeRating AgeRating { get; set; }
        public string? BookCoverUrl { get; set; }
    }

    public class CreateBookDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string AuthorId { get; set; } = null!;
        public ICollection<int> GenresId { get; set; } = null!;
        public ICollection<int> TagsId { get; set; } = null!;
        public AgeRating AgeRating { get; set; }
        public string? BookCoverUrl { get; set; }
        public string? BookPanoramUrl { get; set; }
    }

    public class PopularBook
    {
        public int BookId { get; set; }
        public string? BookCoverUrl { get; set; }
    }

    public class MainPopularBook
    {
        public int BookId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string AuthorId { get; set; } = null!;
        public string AuthorUserName { get; set; } = null!;
        public ICollection<GenreDTO> Genres { get; set; } = null!;
        public ICollection<TagDTO> Tags { get; set; } = null!;
        public double Rating { get; set; }
        public int Endings { get; set; }
        public int Choices { get; set; }
        public string? BookCoverUrl { get; set; }
        public string? BookPanoramUrl { get; set; }
        public DateTime PublicationData { get; set; }
    }
}
