namespace AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;

public interface IUser
{
    Task Save();
    Task<ICollection<User>> GetUserAsync();
    Task<User?> GetUser(string email);
    Task<User> GetUserByName(string Name);
    Task<bool> CheckUserPassword(User user, string password);
    Task<IdentityResult> CreateUser(User newUser, string password);
    Task<(bool, User)> CreateUser(GoogleJsonWebSignature.Payload info); 
}

