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
        public async Task<User> FindCommentatorById(string id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<Book> FindBookById(int id)
        {
            return await _context.Books.FirstOrDefaultAsync(x => x.BookId == id);
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
            return await _context.Comments.Take(_context.Comments.Count()).ToListAsync();
        }

        public async Task AddComment(Comment newComment)
        {
            await _context.Comments.AddAsync(newComment);
            await _context.SaveChangesAsync();
        }
    }
}
