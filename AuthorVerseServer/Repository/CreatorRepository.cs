using AuthorVerseServer.Data;
using AuthorVerseServer.Data.JsonModels;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using AuthorVerseServer.Models.ContentModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AuthorVerseServer.Repository
{
    public class CreatorRepository : ICreator
    {
        private readonly DataContext _context;

        public CreatorRepository(DataContext context)
        {
            _context = context;
        }

        public Task<int> SaveAsync()
        {
            return _context.SaveChangesAsync();
        }

        public ValueTask<EntityEntry> AddContentAsync<T>(T content)
        {
            return _context.AddAsync(content!);
        }
        
        public void DeleteContent(ContentBase content)
        {
            _context.Remove(content);
        }

        public void DeleteSection(ChapterSection chapter)
        {
            _context.ChapterSections.Remove(chapter);
        }

        public void DeleteChoices(ICollection<SectionChoice> sectionChoices)
        {
            _context.RemoveRange(sectionChoices);
        }

        public Task<ChapterSection> GetSectionAsync(int chapterId, int number, int flow)
        {
            var value = _context.ChapterSections
                .Include(c => c.ContentBase)
                .Include(c => c.SectionChoices)
                .SingleAsync(c => c.BookChapterId == chapterId && c.Number == number && c.ChoiceFlow == flow);
            
            return value;
        }

        /// <summary>
        /// Check a new section can be added to the db
        /// </summary>
        /// <param name="chapterId"></param>
        /// <param name="flow">The flow that describe current user choice</param>
        /// <returns></returns>
        public Task<int> CheckAddingNewSectionAsync(int chapterId, int flow)
        {
            var value = _context.ChapterSections
                .Where(c => c.BookChapterId == chapterId)
                .Where(c => c.ChoiceFlow == flow)
                .MaxAsync(c => c.Number);
            return value;
        }

        public Task<bool> CheckUpdatingNewSectionAsync(int chapterId, int number, int flow)
        {
            return _context.ChapterSections
                .Where(c => c.BookChapterId == chapterId)
                .Where(c => c.Number == number)
                .Where(c => c.ChoiceFlow == flow)
                .AnyAsync();
        }

        public Task<ExistSection> CheckAddingNewChoiceAsync(int chapterId, int number, int flow)
        {
            var value = _context.ChapterSections
                .Include(c => c.SectionChoices)
                .Where(c => c.BookChapterId == chapterId)
                .Where(c => c.Number == number)
                .Where(c => c.ChoiceFlow == flow)
                .Select(c => new ExistSection()
                {
                    Choices = c.SectionChoices != null 
                            ? c.SectionChoices.Select(sc => sc.ChoiceNumber)
                            .ToList() : new List<int>()
                });

            return value.SingleAsync();
        }

        public Task<bool> CheckExistNextSectionAsync(int chapterId, int number, int flow)
        {
            var value = _context.ChapterSections
                .Where(c => c.BookChapterId == chapterId)
                .Where(c => c.Number == number)
                .Where(c => c.ChoiceFlow == flow)
                .AnyAsync();

            return value;
        }
    }
}
