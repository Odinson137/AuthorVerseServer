using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Http.HttpResults;
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
        public async Task<string> FindCommentatorById(string id)
        {
            User user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            return user.UserName;

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

        public async Task<bool> DeleteComment(int commentId, string userID)
        {
            Comment commentToRemove = await _context.Comments.FirstOrDefaultAsync(x => x.CommentId == commentId);
            if (commentToRemove.Commentator.UserName == userID || userID == "admin")
            {
                _context.Comments.Remove(commentToRemove);
                await _context.SaveChangesAsync();
            }
            else
                return false;
            return true;
        }
    }
}
