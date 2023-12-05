using AuthorVerseServer.Data;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Repository
{
    public class NoteRepository : INote
    {
        private readonly DataContext _context;
        public NoteRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Note>> GetNoteAsync()
        {
            return await _context.Notes.OrderBy(n => n.BaseId).ToListAsync();
        }
    }
}

