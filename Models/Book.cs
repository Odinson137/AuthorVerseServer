﻿using AuthorVerseServer.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    [Index(nameof(BookId), nameof(Title), nameof(Genres), nameof(PublicationData))]
    public class Book
    {
        public int BookId { get; set; }
        [MaxLength(200)]
        public string Title { get; set; } = null!;
        [MaxLength(1000)]
        public string Description { get; set; } = null!;
        public ICollection<Genre> Genres { get; set; } = null!;
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<BookChapter> BookChapters { get; set; } = null!;
        public DateOnly PublicationData { get; set; } // можно наверное убрать и просто смотреть на дату загрузки первой главыи данной книги
        public AgeRating AgeRating { get; set; }
        public Image? BookCover { get; set; } 
        public PublicationPermission Permission { get; set; }
    }
}