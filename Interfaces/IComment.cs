namespace AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;

public interface IComment
{
    Task<ICollection<Comment>> GetCommentAsync();
}

