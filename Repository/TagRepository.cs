using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AuthorVerseServer.Repository
{
    public class TagRepository : ITag
    {
        private readonly DataContext _context;
        public TagRepository(DataContext context)
        {
            _context = context;
        }
        public async Task AddTag(string name)
        {
            await _context.Tags.AddAsync(new Tag()
            {
                Name = name
            });
        }

        public async Task<ICollection<TagDTO>> GetTagAsync()
        {
            var tags = _context.Tags.Select(tag => new TagDTO
            {
                TagId = tag.TagId,
                Name = tag.Name,
            });

            return await tags.ToListAsync();
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
