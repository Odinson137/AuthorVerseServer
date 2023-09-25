namespace AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;

public interface INote
{
    Task<ICollection<Note>> GetNoteAsync();
}

