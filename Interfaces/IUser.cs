namespace AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;

public interface IUser
{
    Task<ICollection<User>> GetUserAsync();
}

