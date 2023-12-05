using AuthorVerseServer.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class Comment : CommentBase
    {

        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
        [Range(1, 5)]
        public int ReaderRating { get; set; }
        public ICollection<CommentRating> CommentRatings { get; set; } = new List<CommentRating>();
    }
}
