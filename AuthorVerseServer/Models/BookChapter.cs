﻿using AuthorVerseServer.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class BookChapter
    {
        [Key]
        public int BookChapterId { get; set; }
        public int BookChapterNumber { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ActionPlace { get; set; }
        public ICollection<Character> Characters { get; set; } = new List<Character>();
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
        public ICollection<Note> Notes { get; set; } = new List<Note>();
        public ICollection<ChapterSection> ChapterSections { get; set; } = new List<ChapterSection>();
        public DateTime PublicationData { get; set; } = DateTime.Now;
        public PublicationType PublicationType { get; set; } = PublicationType.Publicated; // только во время производства
    }
}
