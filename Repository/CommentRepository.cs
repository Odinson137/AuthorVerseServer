using AuthorVerseServer.Data;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Repository
{
    public class CommentRepository : IComment
    {
        private readonly DataContext _context;
        public CommentRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Comment>> GetCommentAsync()
        {
            return await _context.Comments.OrderBy(c => c.CommentId).ToListAsync();
        }
    }
}
