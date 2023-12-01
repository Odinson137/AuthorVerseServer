﻿using AuthorVerseServer.Data;
using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AuthorVerseServer.Repository
{
    public class AccountRepository : IAccount
    {
        private readonly DataContext _context;
        public AccountRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ICollection<UpdateAccountBook>> CheckUserUpdatesAsync(string userId)
        {
            var books = _context.UserSelectedBooks
                .AsNoTracking()
                .Where(book => book.UserId == userId)
                .Where(book => book.LastBookChapterNumber != book.Book.BookChapters.Max(chapter => chapter.BookChapterNumber))
                .Select(book => new UpdateAccountBook
                {
                    BookId = book.BookId,
                    BookTitle = book.Book.Title,
                    ChapterNumber = book.LastBookChapterNumber,
                    BookCoverUrl = book.Book.BookCover,
                });

            return await books.ToListAsync();
        }

        public async Task<UserProfileDTO> GetUserAsync(string userId)
        {
            return await _context.Users.Where(data => data.Id == userId).Select(data => new UserProfileDTO
            {
                UserName = data.UserName,
                Description = data.Description,
                LogoUrl = data.LogoUrl,
            }).FirstOrDefaultAsync();
        }

        private async Task<IQueryable<CommentProfileDTO>> GetQueryUserCommentsAsync(CommentType commentType, string searchComment, string userId)
        {
            if (commentType == CommentType.Chapter)
            {
                var query = await _context.Notes.Where(x => x.UserId == userId)
                   .Select(x => new CommentProfileDTO
                   {
                       CommentId = x.NoteId,
                       Text = x.Text,
                       Likes = x.Likes,
                       DisLikes = x.DisLikes,
                       CommentType = CommentType.Chapter,
                       BookTitle = x.BookChapter.Book.Title,
                       ChapterNumber = x.BookChapter.BookChapterNumber,
                       ChapterTitle = x.BookChapter.Title,
                       CommentCreatedDateTime = DateOnly.FromDateTime(x.NoteCreatedDateTime)
                   }).ToListAsync();
                return query.AsQueryable();
            }
            else if (commentType == CommentType.Book)
            {
                var query = await _context.Comments.Where(x => x.CommentatorId == userId)
                    .Select(x => new CommentProfileDTO
                    {
                        CommentId = x.CommentId,
                        Text = x.Text,
                        Rating = x.ReaderRating,
                        Likes = x.Likes,
                        DisLikes = x.DisLikes,
                        CommentType = CommentType.Book,
                        BookTitle = x.Book.Title,
                        CommentCreatedDateTime = DateOnly.FromDateTime(x.CommentCreatedDateTime)
                    }).ToListAsync();
                return query.AsQueryable();
            }
            else if (!string.IsNullOrEmpty(searchComment))
            {
                var query = await _context.Comments.Where(x => x.CommentatorId == userId)
                .Select(x => new CommentProfileDTO
                {
                    CommentId = x.CommentId,
                    Text = x.Text,
                    Rating = x.ReaderRating,
                    Likes = x.Likes,
                    DisLikes = x.DisLikes,
                    CommentType = CommentType.Book,
                    BookTitle = x.Book.Title,
                    CommentCreatedDateTime = DateOnly.FromDateTime(x.CommentCreatedDateTime)
                }).Union(_context.Notes.Where(x => x.UserId == userId)
                .Where(x => x.UserId == userId)
                .Select(x => new CommentProfileDTO
                {
                    CommentId = x.NoteId,
                    Text = x.Text,
                    Likes = x.Likes,
                    DisLikes = x.DisLikes,
                    CommentType = CommentType.Chapter,
                    BookTitle = x.BookChapter.Book.Title,
                    ChapterNumber = x.BookChapter.BookChapterNumber,
                    ChapterTitle = x.BookChapter.Title,
                    CommentCreatedDateTime = DateOnly.FromDateTime(x.NoteCreatedDateTime)
                })).Where(x => x.Text == searchComment).ToListAsync();
                return query.AsQueryable();
            }
            else
            {//CommentType == CommentType.All
                var query = await _context.Comments.Where(x => x.CommentatorId == userId)
                .Select(x => new CommentProfileDTO
                {
                    CommentId = x.CommentId,
                    Text = x.Text,
                    Rating = x.ReaderRating,
                    Likes = x.Likes,
                    DisLikes = x.DisLikes,
                    CommentType = CommentType.Book,
                    BookTitle = x.Book.Title,
                    CommentCreatedDateTime = DateOnly.FromDateTime(x.CommentCreatedDateTime)
                }).Union(_context.Notes.Where(x => x.UserId == userId)
                .Where(x => x.UserId == userId)
                .Select(x => new CommentProfileDTO
                {
                    CommentId = x.NoteId,
                    Text = x.Text,
                    Likes = x.Likes,
                    DisLikes = x.DisLikes,
                    CommentType = CommentType.Chapter,
                    BookTitle = x.BookChapter.Book.Title,
                    ChapterNumber = x.BookChapter.BookChapterNumber,
                    ChapterTitle = x.BookChapter.Title,
                    CommentCreatedDateTime = DateOnly.FromDateTime(x.NoteCreatedDateTime)
                })).ToListAsync();
                return query.AsQueryable();
            }
        }

        public async Task<int> GetCommentsPagesCount(CommentType commentType, string searchComment, string userId)
        {//Пользователя
            IQueryable<CommentProfileDTO> query = await GetQueryUserCommentsAsync(commentType, searchComment, userId);
            return await query.CountAsync();
        }

        public async Task<ICollection<CommentProfileDTO>> GetUserCommentsAsync(CommentType commentType, int page, string searchComment, string userId)
        {
            IQueryable<CommentProfileDTO> query = await GetQueryUserCommentsAsync(commentType, searchComment, userId);
            return await query
                .AsNoTracking()
                .OrderByDescending(x => x.CommentCreatedDateTime)
                .Skip(page * 5)
                .Take(5)
                .ToListAsync();
        }

        public async Task<ICollection<UserBookDTO>> GetUserBooksAsync(string userId)
        {
            var books = _context.Books
                .Where(book => book.AuthorId == userId)
                .Select(book => new UserBookDTO
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Description = book.Description,
                    BookCoverUrl = book.BookCover,
                    PublicationData = DateOnly.FromDateTime(book.PublicationData),
                    Earnings = book.Earnings
                });

            return await books.ToListAsync();
        }


        public Task<ICollection<FriendDTO>> GetUserFriendsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<SelectedUserBookDTO>> GetUserSelectedBooksAsync(string userId)
        {
            return await _context.UserSelectedBooks
                .Where(data => data.UserId == userId)
                .Select(data=> new SelectedUserBookDTO
                {
                    BookId = data.BookId,
                    Title = data.Book.Title,
                    ImageUrl = data.Book.BookCover,
                    BookState = data.BookState,
                    PublicationData = DateOnly.FromDateTime(data.Book.PublicationData.Date),
                    LastReadingChapter = data.LastBookChapterNumber,
                    LastBookChapter = data.Book.BookChapters.Max(x=> x.BookChapterNumber),
                }).ToListAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}