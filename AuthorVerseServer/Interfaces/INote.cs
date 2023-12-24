namespace AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;

public interface INote
{
    Task<List<Note>> GetNoteAsync();
}

