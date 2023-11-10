﻿namespace AuthorVerseServer.Interfaces;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;

public interface IComment
{
    Task<Comment?> GetCommentAsync(int bookId);
    Task<Book?> GetBook(int bookId);
    Task<Comment?> CheckUserComment(Book book, User user);
    Task AddComment(Comment newComment);
    Task<bool> DeleteComment(int commentId);
    Task<int> Save();
}

