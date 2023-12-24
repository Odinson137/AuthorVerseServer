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

        public Task AddBookQuoteAsync(BookQuote quote)
        {
            _context.BookQuotes.AddAsync(quote);
            return Task.CompletedTask;
        }

        public Task DeleteBookQuoteAsync(int deleteQuoteId)
        {
            _context.BookQuotes.Where(quote => quote.BaseId == deleteQuoteId).ExecuteDeleteAsync();
            return Task.CompletedTask;
        }

        public Task<List<QuoteDTO>> GetBookQuotesAsync(int bookId, int page)
        {
            var quotes = _context.BookQuotes
                .OrderByDescending(book => book.BaseId)
                .Where(quote => quote.BookId == bookId)
                .Skip(5 * page)
                .Take(5)
                .Select(quote => new QuoteDTO
                {
                    QuoteId = quote.BaseId,
                    Text = quote.Text,
                    User = new UserDTO
                    {
                        Id = quote.UserId,
                        UserName = quote.User.UserName
                    },
                    LikeCount = quote.CommentRatings.Count(x => x.LikeRating == Data.Enums.LikeRating.Like),
                    DisLikesCount = quote.CommentRatings.Count(x => x.LikeRating == Data.Enums.LikeRating.DisLike),
                    //LikeCount = quote.Likes,
                    //DisLikesCount = quote.DisLikes,
                    QuoteCreatedDateTime = DateOnly.FromDateTime(quote.CreatedDateTime)
                }).ToListAsync();

            return quotes;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
