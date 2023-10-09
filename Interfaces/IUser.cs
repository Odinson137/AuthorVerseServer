namespace AuthorVerseServer.Interfaces;

using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;
using Google.Apis.Auth;

public interface IUser
{
    Task Save();
    Task<ICollection<User>> GetUserAsync();
    Task<User?> GetUserByEmail(string email);
    Task<User?> GetUserByUserName(string userName);
    //Task<(bool, User)> CreateUser(GoogleJsonWebSignature.Payload info); 
    Task<(bool, User)> CreateGoogleUser(GoogleJsonWebSignature.Payload info);
    Task<(bool, User)> CreateMicrosoftUser(UserProfile info);
}

