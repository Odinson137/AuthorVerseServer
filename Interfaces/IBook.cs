namespace AuthorVerseServer.Interfaces;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;
using MailKit.Search;
using Microsoft.EntityFrameworkCore.Storage;

public interface IBook
{
    Task Save();
    Task<ICollection<Book>> GetBooksAsync();
    Task<int> GetCountBooks();
    Task<ICollection<PopularBook>> GetPopularBooks();
    Task<ICollection<PopularBook>> GetLastBooks();
    Task<ICollection<BookDTO>> GetCertainBooksPage(int tag, int genre, int page, string searchText);
    Task<int> GetBooksCountByTagsAndGenres(int tagId, int genreId, string searchText);
    Task<ICollection<MainPopularBook>> GetMainPopularBook();
    Task<DetailBookDTO?> GetBookById(int bookId);
    Task AddBook(Book book);
    Task<Genre?> GetGenreById(int id);
    Task<Tag?> GetTagById(int id);
    Task<ICollection<AuthorMinimalBook>> GetAuthorBooksAsync(string userId);
    Task<ICollection<QuoteDTO>> GetBookQuotesAsync(int bookId, int page);
}

