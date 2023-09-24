using AuthorVerseServer.Data;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Repository
{
    public class BookChapterRepository: IBookChapter
    {
        private readonly DataContext context;
        public BookChapterRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<ICollection<BookChapter>> GetBookChapterAsync()
        {
            return await context.BookChapters.OrderBy(p => p.BookId).ToListAsync();
        }


    }
}

