using AuthorVerseServer.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthorVerseServer.Models
{
    [Index(nameof(Title)), Index(nameof(PublicationData)), Index(nameof(AverageRating))]
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        [MaxLength(200)]
        public string Title { get; set; } = null!;
        [MaxLength(1000)]
        public string Description { get; set; } = null!;
        public string AuthorId { get; set; } = null!;
        public User Author { get; set; } = null!;
        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<BookChapter> BookChapters { get; set; } = new List<BookChapter>();
        public DateTime PublicationData { get; set; } // можно наверное убрать и просто смотреть на дату загрузки первой главыи данной книги
        public double AverageRating { get; set; }
        public AgeRating AgeRating { get; set; }
        public Image? BookCover { get; set; } 
        public PublicationPermission Permission { get; set; }
    }
}
