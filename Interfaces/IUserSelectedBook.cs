namespace AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;

public interface IUserSelectedBook
{
    Task<ICollection<UserSelectedBook>> GetUserSelectedBookAsync();
}

