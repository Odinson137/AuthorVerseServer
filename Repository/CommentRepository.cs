using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AuthorVerseServer.Repository
{
    public class CommentRepository : IComment
    {
        private readonly DataContext _context;
        public CommentRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Comment?> CheckUserComment(Book book, User user)
        {
            return await _context.Comments.Where(x => x.Book == book && x.Commentator == user).FirstOrDefaultAsync();
        }

        public async Task<Book?> GetBook(int bookId)
        {
            return await _context.Books.FirstOrDefaultAsync(x => x.BookId == bookId);
        }

        public async Task AddComment(Comment newComment)
        {
            await _context.Comments.AddAsync(newComment);
        }

        public async Task<bool> DeleteComment(int commentId)
        {
            Comment? commentToRemove = await _context.Comments.FirstOrDefaultAsync(x => x.CommentId == commentId);
            //if (commentToRemove.Commentator.UserName == userID || userID == "admin")
            //{
            //_context.Comments.Remove(commentToRemove);
            //    await _context.SaveChangesAsync();
            //}
            //else
            //    return false;
            return true;
        }

        public async Task<Comment?> GetCommentAsync(int commentId)
        {
            return await _context.Comments.FirstOrDefaultAsync(x=> x.CommentId == commentId);
        }

        public async Task<Comment?> GetUserCommentAsync(string commentatorId, int bookId)
        {
            return await _context.Comments.FirstOrDefaultAsync(x => x.BookId == bookId && x.CommentatorId == commentatorId);
        }

        public async Task<int> Save()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
