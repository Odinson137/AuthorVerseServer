using AuthorVerseServer.Data;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Repository
{
    public class CharacterRepository : ICharacter
    {
        private readonly DataContext _context;
        public CharacterRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Character>> GetCharacterAsync()
        {
            return await _context.Characters.OrderBy(g => g.CharacterId).ToListAsync();
        }
    }
}
