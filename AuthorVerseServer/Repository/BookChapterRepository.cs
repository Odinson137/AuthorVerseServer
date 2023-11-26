using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Repository
{
    public class BookChapterRepository: IBookChapter
    {
        private readonly DataContext _context;
        public BookChapterRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ICollection<BookChapterDTO>> GetBookChapterAsync()
        {
            return await _context.BookChapters.OrderBy(p => p.BookId).Select(x => new BookChapterDTO()
            {
                BookChapterId = x.BookChapterId,
                BookId = x.BookId,
                Description = x.Description,
                PublicationData = x.PublicationData,
            }
            ).ToListAsync();
        }
    }
}

