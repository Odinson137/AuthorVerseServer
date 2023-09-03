﻿using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class Genre
    {
        [Key]
        public int GenreId { get; set; }
        public string Name { get; set; } = null!;
    }
}
