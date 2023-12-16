using AuthorVerseServer.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class UserSelectedBook
    {
        [Key]
        public int UserBookId { get; set; }
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
        public int LastBookChapterNumber { get; set; }
        public BookState BookState { get; set; }
    }
}
