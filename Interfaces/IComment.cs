namespace AuthorVerseServer.Interfaces;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;

public interface IComment
{
    Task<ICollection<Comment>> GetCommentAsync();
    Task<Book?> GetBook(int bookId);
    Task<Comment?> CheckUserComment(Book book, User user);
    Task<string> FindCommentatorById(string id);
    Task<Book> FindBookById(int id);
    Task AddComment(Comment newComment);
    Task<bool> DeleteComment(int commentId, string userId);
}

