using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.Models;
using Microsoft.Extensions.FileProviders;
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
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200)]
        public string Title { get; set; } = null!;
        [Required(ErrorMessage = "Description is required")]
        [MinLength(50, ErrorMessage = "Description length is lower than 50 letter")]
        [MaxLength(1000, ErrorMessage = "Description length more than 1000 characters")]
        public string Description { get; set; } = null!;
        [Required(ErrorMessage = "Author Id is required")]
        public string AuthorId { get; set; } = null!;
        [Required(ErrorMessage = "Genre is required")]
        public ICollection<int> GenresId { get; set; } = null!;
        [Required(ErrorMessage = "Tag is required")]
        public ICollection<int> TagsId { get; set; } = null!;
        [Required(ErrorMessage = "Age rating is required")]
        public AgeRating AgeRating { get; set; }
        public IFormFile? BookCoverImage { get; set; }
        public IFormFile? BookPanoramImage { get; set; }
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
