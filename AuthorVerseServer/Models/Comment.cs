using AuthorVerseServer.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class Comment : CommentBase
    {
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
        public int? BookId { get; set; }
        public Book? Book { get; set; } = null!;
        [Range(1, 5)]
        public int ReaderRating { get; set; }
    }
}
