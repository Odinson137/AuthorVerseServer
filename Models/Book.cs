using AuthorVerseServer.Data;
using AuthorVerseServer.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        [MaxLength(200)]
        public string Title { get; set; } = null!;
        public string NormalizedTitle { get; set; } = null!;
        [MaxLength(1000)]
        public string Description { get; set; } = null!;
        public string AuthorId { get; set; } = null!;
        public User Author { get; set; } = null!;
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Character> Characters { get; set; } = new List<Character>();
        public ICollection<BookChapter> BookChapters { get; set; } = new List<BookChapter>();
        public ICollection<UserSelectedBook> UserSelectedBooks { get; set; } = new List<UserSelectedBook>(); // наверное нужно убрать
        public DateTime PublicationData { get; } = DateTime.Now; // можно наверное убрать и просто смотреть на дату загрузки первой главыи данной книги
        public AgeRating AgeRating { get; set; }
        public string? BookCover { get; set; }
        [Range(1, 5)]
        public double Rating { get; set; }
        public int CountRating { get; set; }
        public PublicationPermission Permission { get; set; }
    }
}
