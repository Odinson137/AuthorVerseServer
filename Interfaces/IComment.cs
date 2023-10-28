namespace AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;

public interface IComment
{
    Task<ICollection<Comment>> GetCommentAsync();
    Task<Book?> GetBook(int bookId);
    Task<Comment?> CheckUserComment(Book book, User user);
    Task AddComment();
}

