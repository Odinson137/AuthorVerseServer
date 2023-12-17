using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;

namespace AuthorVerseServer.Interfaces;

public interface ICharacter
{
    Task<bool> IsAuthorByCharacterIdAsync(int characterId, string userId);
    Task<bool> IsAuthorByChapterIdAsync(int chapterId, string userId);
    Task<bool> IsAuthorByBookIdAsync(int bookId, string userId);
    Task<Character?> GetCharacterAsync(int characterId);
    Task<ICollection<BookCharacterDTO>> GetCharactersAsync(int bookId);
    Task<ICollection<BookCharacterDTO>> GetCharactersByNameAsync(int bookId, string name);
    Task<bool> ExistNameAsync(int bookId, string name);
    Task<bool> ExistCharacterChaption(int chapterId, int characterId);
    Task AddCharacterToBookAsync(Character character);
    Task AddCharacterToChapterAsync(CharacterChapter characterChapter);
    Task DeleteCharacterFromBookAsync(int characterId);
    Task DeleteCharacterFromChaptersAsync(int characterId);
    Task DeleteCharacterFromChapterAsync(int chapterId, int characterId);
    Task SaveAsync();
}

