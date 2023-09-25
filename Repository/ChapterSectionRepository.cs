using AuthorVerseServer.Data;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Repository
{
    public class ChapterSectionRepository : IChapterSection
    {
        private readonly DataContext _context;
        public ChapterSectionRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ICollection<ChapterSection>> GetChapterSectionAsync()
        {
            return await _context.ChapterSections.OrderBy(cs => cs.SectionId).ToListAsync();
        }
    }
}
