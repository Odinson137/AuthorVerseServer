namespace AuthorVerseServer.Interfaces;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;

public interface IComment
{
    Task<ICollection<Comment>> GetCommentsByBookAsync(int bookId);
    Task<Comment?> GetCommentAsync(int commentId);
    Task<Book?> GetBook(int bookId);
    Task<Comment?> CheckUserComment(Book book, User user);
    Task AddComment(Comment newComment);
    Task DeleteComment(Comment commentToRemove);
    Task<int> Save();
}

