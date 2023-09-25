﻿using AuthorVerseServer.Data;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Repository
{
    public class BookChapterRepository: IBookChapter
    {
        private readonly DataContext _context;
        public BookChapterRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ICollection<BookChapter>> GetBookChapterAsync()
        {
            return await _context.BookChapters.OrderBy(bc => bc.BookId).ToListAsync();
        }
    }
}

