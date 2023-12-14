namespace AuthorVerseServer.Interfaces;

using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;

public interface IUser
{
    Task Save();
    Task<ICollection<string>> GetUserEmailAsync(int bookId);
    Task CreateMicrosoftUser(MicrosoftUser microsoftUser);
    Task<MicrosoftUser?> GetMicrosoftUser(string azureName);
}
