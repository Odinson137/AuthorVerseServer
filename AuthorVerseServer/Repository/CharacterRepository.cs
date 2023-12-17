using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AuthorVerseServer.Repository
{
    public class CharacterRepository : ICharacter
    {
        private readonly DataContext _context;
        public CharacterRepository(DataContext context)
        {
            _context = context;
        }

        public async Task AddCharacterToBookAsync(Character character)
        {
            await _context.Characters.AddAsync(character);
        }

        public async Task AddCharacterToChapterAsync(CharacterChapter characterChapter)
        {
            await _context.CharacterChapters.AddAsync(characterChapter);
        }

        public async Task DeleteCharacterFromBookAsync(int characterId)
        {
            await _context.Characters
                .Where(c => c.CharacterId == characterId)
                .ExecuteDeleteAsync();
        }

        public async Task DeleteCharacterFromChaptersAsync(int characterId)
        {
            await _context.CharacterChapters
                .Where(c => c.CharacterId == characterId)
                .ExecuteDeleteAsync();
        }

        public async Task DeleteCharacterFromChapterAsync(int chapterId, int characterId)
        {
            await _context.CharacterChapters
                .Where(c => c.ChapterId == chapterId)
                .Where(c => c.CharacterId == characterId)
                .ExecuteDeleteAsync();
        }

        public async Task<Character?> GetCharacterAsync(int characterId)
        {
            var character = await _context.Characters
                .Where(c => c.CharacterId == characterId)
                .FirstOrDefaultAsync();

            return character;
        }

        public async Task<ICollection<BookCharacterDTO>> GetCharactersAsync(int bookId)
        {
            var characters = _context.Characters
                .Where(c => c.BookId == bookId)
                .Select(c => new BookCharacterDTO
                {
                    CharacterId = c.CharacterId,
                    Name = c.Name,
                    Description = c.Description,
                    Url = c.CharacterImageUrl,
                });

            return await characters.ToListAsync();
        }

        public async Task<ICollection<BookCharacterDTO>> GetCharactersByNameAsync(int bookId, string name)
        {
            var characters = _context.Characters
                .Where(c => c.BookId == bookId)
                .Where(c => c.NormalizedName.Contains(name.ToUpper()))
                .Select(c => new BookCharacterDTO
                {
                    CharacterId = c.CharacterId,
                    Name = c.Name,
                    Description = c.Description,
                    Url = c.CharacterImageUrl,
                });

            return await characters.ToListAsync();
        }

        public async Task<bool> IsAuthorByCharacterIdAsync(int characterId, string userId)
        {
            return await _context.Characters
                .Where(b => b.CharacterId == characterId)
                .AnyAsync(b => b.Book.AuthorId == userId);
        }
        public async Task<bool> IsAuthorByBookIdAsync(int bookId, string userId)
        {
            return await _context.Books
                .Where(b => b.BookId == bookId)
                .AnyAsync(b => b.AuthorId == userId);
        }

        public async Task<bool> IsAuthorByChapterIdAsync(int chapterId, string userId)
        {
            return await _context.BookChapters
                .Where(b => b.BookChapterId == chapterId)
                .AnyAsync(b => b.Book.AuthorId == userId);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistNameAsync(int bookId, string name)
        {
            return await _context.Characters
                .Where(c => c.BookId == bookId)
                .AnyAsync(c => c.NormalizedName == name.ToUpper());
        }

        public async Task<bool> ExistCharacterChaption(int chapterId, int characterId)
        {
            return await _context.CharacterChapters
                .Where(cc => cc.ChapterId == chapterId)
                .Where(cc => cc.CharacterId == characterId)
                .AnyAsync();
        }

    }
}
