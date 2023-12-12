using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AuthorVerseServer.Repository
{
    public class QuoteRepository : IQuote
    {
        private readonly DataContext _context;
        public QuoteRepository(DataContext context)
        {
            _context = context;
        }

        public async Task AddBookQuoteAsync(BookQuote quote)
        {
            await _context.BookQuotes.AddAsync(quote);
        }

        public async Task DeleteBookQuoteAsync(int deleteQuoteId)
        {
            await _context.BookQuotes.Where(quote => quote.BookQuotesId == deleteQuoteId).ExecuteDeleteAsync();
        }

        public async Task<ICollection<QuoteDTO>> GetBookQuotesAsync(int bookId, int page)
        {
            var quotes = await _context.BookQuotes
                .AsNoTracking()
                .OrderByDescending(book => book.Likes)
                .Where(quote => quote.BookId == bookId)
                .Skip(5 * page)
                .Take(5)
                .Select(quote => new QuoteDTO
                {
                    QuoteId = quote.BookQuotesId,
                    Text = quote.Text,
                    Quoter = new UserDTO
                    {
                        Id = quote.QuoterId,
                        UserName = quote.Quoter.UserName
                    },
                    LikeCount = quote.QuoteRatings.Count(x => x.Rating == Data.Enums.LikeRating.Like),
                    DisLikesCount = quote.QuoteRatings.Count(x => x.Rating == Data.Enums.LikeRating.DisLike),
                    //LikeCount = quote.Likes,
                    //DisLikesCount = quote.DisLikes,
                    QuoteCreatedDateTime = DateOnly.FromDateTime(quote.QuoteCreatedDateTime)
                }).ToListAsync();

            return quotes;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
