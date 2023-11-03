﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;

namespace AuthorVerseServer.Models
{
    public class Character
    {
        [Key]
        public int CharacterId { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
        public int BookChapterId { get; set; }
        public string? Description { get; set; }
        public string? CharacterImageUrl { get; set; }
    }
}
