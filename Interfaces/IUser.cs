namespace AuthorVerseServer.Interfaces;

using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;

public interface IUser
{
    Task Save();
    Task<ICollection<User>> GetUserAsync();
    //Task<bool> CreateForeignUser(User user);
    //Task<User?> GetUserByEmail(string email);
    //Task<User?> GetUserByUserName(string userName);
    Task CreateMicrosoftUser(MicrosoftUser microsoftUser);
    Task<MicrosoftUser?> GetMicrosoftUser(string azureName);
    //Task<bool> CheckUserPassword(User user, string password);
    //Task<IdentityResult> CreateUser(User newUser, string password);
}

