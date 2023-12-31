﻿namespace AuthorVerseServer.Models
{
    public class Note : CommentBase
    {
        public int? BookChapterid { get; set; }
        public BookChapter? BookChapter { get; set; } = null!;
        public int? ReplyToBaseId { get; set; }
        public Note? ReplyNote { get; set; }
        public ICollection<Note> Replies { get; set; } = new List<Note>();
    }
}
