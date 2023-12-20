using AuthorVerseServer.Data;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Repository
{
    public class SectionChoiceRepository : ISectionChoice
    {
        private readonly DataContext _context;
        public SectionChoiceRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ICollection<SectionChoice>> GetSectionChoiceAsync()
        {
            return await _context.SectionChoices.ToListAsync();
        }
    }
}
