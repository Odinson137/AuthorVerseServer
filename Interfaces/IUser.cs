namespace AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Google.Apis.Auth;

public interface IUser
{
    Task Save();
    Task<ICollection<User>> GetUserAsync();
    Task<User?> GetUser(string email);
    Task<(bool, User)> CreateUser(GoogleJsonWebSignature.Payload info); 
}

