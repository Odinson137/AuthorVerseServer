using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Tag = AuthorVerseServer.Models.Tag;

namespace AuthorVerseServer.Repository
{
    public class TagRepository : ITag
    {
        private readonly DataContext _context;
        public TagRepository(DataContext context)
        {
            _context = context;
        }
        public Task AddTag(string name)
        {
            _context.Tags.AddAsync(new Tag()
            {
                Name = name
            });
            return Task.CompletedTask;
        }

        public Task<List<TagDTO>> GetTagAsync()
        {
            var tags = _context.Tags.Select(tag => new TagDTO
            {
                TagId = tag.TagId,
                Name = tag.Name,
            });

            return tags.ToListAsync();
        }

        public Task<int> SaveAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
