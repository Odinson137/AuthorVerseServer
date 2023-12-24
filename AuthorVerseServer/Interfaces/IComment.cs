namespace AuthorVerseServer.Interfaces;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;

public interface IComment
{
    Task<List<CommentDTO>> GetCommentsByBookAsync(int bookId, int page, string? userId = null);
    Task<Comment?> GetCommentAsync(int commentId);
    Task<bool> CheckExistCommentAsync(int bookId, string userId);
    Task<int> ChechExistBookAsync(int bookId);
    Task AddComment(Comment newComment);
    Task DeleteComment(int commentId);
    Task<int> Save();
}

