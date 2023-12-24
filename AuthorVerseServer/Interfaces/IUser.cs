using AuthorVerseServer.Models;

namespace AuthorVerseServer.Interfaces;
public interface IUser
{
    Task Save();
    Task<List<string>> GetUserEmailAsync(int bookId);
    Task CreateMicrosoftUser(MicrosoftUser microsoftUser);
    Task<MicrosoftUser?> GetMicrosoftUser(string azureName);
}
