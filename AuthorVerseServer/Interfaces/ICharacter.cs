namespace AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;

public interface ICharacter
{
    Task<ICollection<Character>> GetCharacterAsync();
}

