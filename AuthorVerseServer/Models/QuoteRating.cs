using AuthorVerseServer.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class QuoteRating
    {
        [Key]
        public int QuoteRatingId { get; set; }
        public int QuoteId { get; set; }
        public string UserQuotedId { get; set; } = null!;
        public LikeRating Rating { get; set; }
    }
}