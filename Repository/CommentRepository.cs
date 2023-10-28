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

        public Task<Book> AddComment()
        {
            throw new NotImplementedException();
        }

        public Task<Book> CheckUserComment()
        {
            throw new NotImplementedException();
        }

        public Task<Comment> CheckUserComment(Book book, User user)
        {
            throw new NotImplementedException();
        }

        public Task<Book> GetBook()
        {
            throw new NotImplementedException();
        }

        public Task<Book> GetBook(int bookId)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<Comment>> GetCommentAsync()
        {
            return await _context.Comments.OrderBy(c => c.CommentId).ToListAsync();
        }

        Task IComment.AddComment()
        {
            throw new NotImplementedException();
        }
    }
}
