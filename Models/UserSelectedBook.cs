using AuthorVerseServer.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class UserSelectedBook
    {
        [Key]
        public int UserBookId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public BookState BookState { get; set; }
        public int LastBookChapterId { get; set; }
    }
}
