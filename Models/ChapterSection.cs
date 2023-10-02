﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    [Index("BookChapterId")]
    public class ChapterSection
    {
        [Key]
        public int SectionId { get; set; }
        public int BookChapterId { get; set; }
        public BookChapter BookChapter { get; set; } = null!;
        public int Number { get; set; }
        public string? Text { get; set; } // либо это
        public Image? Image { get; set; } // либо это
        public int NextSectionId { get; set; }
        public ICollection<SectionChoice>? SectionChoices { get; set; }
    }
}
