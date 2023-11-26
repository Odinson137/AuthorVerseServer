﻿using AuthorVerseServer.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class Note
    {
        [Key]
        public int NoteId { get; set; }
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
        public int BookChapterid { get; set; }
        public BookChapter BookChapter { get; set; } = null!;
        public string Text { get; set; } = null!;
        public int Likes { get; set; } = 0;
        public int DisLikes { get; set; } = 0;
        public DateTime NoteCreatedDateTime { get; set; } = DateTime.Now;
        public PublicationPermission Permission { get; set; } = PublicationPermission.Approved;
        public int? ReplyToNoteId { get; set; }
        public ICollection<Note> Replies { get; set; } = new List<Note>();
    }
}
