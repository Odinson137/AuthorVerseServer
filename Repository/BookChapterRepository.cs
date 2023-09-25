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
<<<<<<< HEAD
            return await context.BookChapters.OrderBy(p => p.BookId).Select(x => new BookChapterDTO()
            {
                BookChapterId = x.BookChapterId,
                BookId = x.BookId,
                Description = x.Description,
                PublicationData = x.PublicationData,
            }
            ).ToListAsync();
=======
            return await _context.BookChapters.OrderBy(bc => bc.BookId).ToListAsync();
>>>>>>> 98aa98ac915488b23ad69ea3a3d06058f80da620
        }
    }
}

