using AuthorVerseServer.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthorVerseServer.Models
{
    public class UserBook
    {
        [Key]
        public int UserBookId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int BookId { get; set; }
        [ForeignKey("BookId")]
        public Book Book { get; set; } = null!;
        public BookState BookState { get; set; }

        public int LastBookChapterId { get; set; }
        [ForeignKey("LastBookChapterId")]
        public BookChapter? LastBookChapter { get; set; }
    }
}
