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

        public Task AddCharacterToBookAsync(Character character)
        {
            _context.Characters.AddAsync(character);
            return Task.CompletedTask;
        }

        public Task AddCharacterToChapterAsync(CharacterChapter characterChapter)
        {
            _context.CharacterChapters.AddAsync(characterChapter);
            return Task.CompletedTask;
        }

        public Task DeleteCharacterFromBookAsync(int characterId)
        {
            _context.Characters
                .Where(c => c.CharacterId == characterId)
                .ExecuteDeleteAsync();
            return Task.CompletedTask;
        }

        public Task DeleteCharacterFromChaptersAsync(int characterId)
        {
            _context.CharacterChapters
                .Where(c => c.CharacterId == characterId)
                .ExecuteDeleteAsync();
            return Task.CompletedTask;
        }

        public Task DeleteCharacterFromChapterAsync(int chapterId, int characterId)
        {
            _context.CharacterChapters
                .Where(c => c.ChapterId == chapterId)
                .Where(c => c.CharacterId == characterId)
                .ExecuteDeleteAsync();
            return Task.CompletedTask;
        }

        public Task<Character?> GetCharacterAsync(int characterId)
        {
            var character = _context.Characters
                .Where(c => c.CharacterId == characterId)
                .FirstOrDefaultAsync();

            return character;
        }

        public Task<List<BookCharacterDTO>> GetCharactersAsync(int bookId)
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

            return characters.ToListAsync();
        }

        public Task<List<BookCharacterDTO>> GetCharactersByNameAsync(int bookId, string name)
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

            return characters.ToListAsync();
        }

        public Task<bool> IsAuthorByCharacterIdAsync(int characterId, string userId)
        {
            return _context.Characters
                .Where(b => b.CharacterId == characterId)
                .AnyAsync(b => b.Book.AuthorId == userId);
        }
        public Task<bool> IsAuthorByBookIdAsync(int bookId, string userId)
        {
            return _context.Books
                .Where(b => b.BookId == bookId)
                .AnyAsync(b => b.AuthorId == userId);
        }

        public Task<bool> IsAuthorByChapterIdAsync(int chapterId, string userId)
        {
            return _context.BookChapters
                .Where(b => b.BookChapterId == chapterId)
                .AnyAsync(b => b.Book.AuthorId == userId);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public Task<bool> ExistNameAsync(int bookId, string name)
        {
            return _context.Characters
                .Where(c => c.BookId == bookId)
                .AnyAsync(c => c.NormalizedName == name.ToUpper());
        }

        public Task<bool> ExistCharacterChaption(int chapterId, int characterId)
        {
            return _context.CharacterChapters
                .Where(cc => cc.ChapterId == chapterId)
                .Where(cc => cc.CharacterId == characterId)
                .AnyAsync();
        }

    }
}
