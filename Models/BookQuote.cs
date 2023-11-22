using AuthorVerseServer.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class BookQuotes
    {
        [Key]
        public int BookQuotesId { get; set; }
        public string QuoterId { get; set; } = null!;
        public User Quoter { get; set; } = null!;
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
        [MaxLength(1000)]
        public string Text { get; set; } = null!;
        public int Likes { get; set; } = 0;
        public int DisLikes { get; set; } = 0;
        public DateTime QuoteCreatedDateTime { get; } = DateTime.Now;
        public ICollection<QuoteRating> QuoteRatings { get; set; } = new List<QuoteRating>();
        public PublicationPermission Permission { get; set; } = PublicationPermission.Approved;
    }
}
