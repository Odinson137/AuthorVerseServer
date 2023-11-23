using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;

namespace AuthorVerseServer.Interfaces
{
    public interface IQuote
    {
        Task<ICollection<QuoteDTO>> GetBookQuotesAsync(int bookId, int page);
        Task AddBookQuoteAsync(BookQuote quote);
        Task DeleteBookQuoteAsync(BookQuote deleteQuote);
        Task SaveAsync();
    }
}
