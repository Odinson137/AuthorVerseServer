using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;

namespace AuthorVerseServer.Interfaces
{
    public interface IQuote
    {
        Task<List<QuoteDTO>> GetBookQuotesAsync(int bookId, int page);
        Task AddBookQuoteAsync(BookQuote quote);
        Task DeleteBookQuoteAsync(int deleteQuoteId);
        Task<int> SaveAsync();
    }
}
