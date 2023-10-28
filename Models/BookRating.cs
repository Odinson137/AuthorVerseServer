using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    [Index("BookId"), Index("Rating")]
    public class BookRating
    {
        [Key]
        public int BookRatingId { get; set; }
        public int BookId { get; set; }
        public int Rating { get; set; }
    }
}
