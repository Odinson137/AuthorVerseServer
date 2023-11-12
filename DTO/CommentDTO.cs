using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.DTO
{
    public class CommentDTO
    {
        public int CommentId { get; set; }
        public UserDTO Commentator { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateOnly CommentCreatedDateTime { get; set; }
    }

    public class CommentProfileDTO
    {
        public int CommentId { get; set; }
        public string Text { get; set; } = null!;
        public int Rating { get; set; }
        public CommentType CommentType { get; set; }
        public string BookTitle { get; set; } = null!;
        public int ChapterNumber { get; set; }
        public string? ChapterTitle { get; set; }
        public DateOnly CommentCreatedDateTime { get; set; }
    }

    public class CreateCommentDTO
    {
        [Required]
        public string UserId { get; set; } = null!;
        public int BookId { get; set; }
        [Required]
        [MaxLength(400, ErrorMessage = "Messege's too big")]
        [MinLength(50, ErrorMessage = "Messege's too short")]
        public string Text { get; set; } = null!;
    }
}
