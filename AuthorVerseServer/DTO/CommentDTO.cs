using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.DTO
{
    public class CommentDTO
    {
        public int BaseId { get; set; }
        public UserDTO User { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateOnly CreatedDateTime { get; set; }
    }


    public class CreateCommentDTO
    {
        public int BookId { get; set; }
        [Required]
        [MaxLength(400, ErrorMessage = "Messege's too big")]
        [MinLength(50, ErrorMessage = "Messege's too short")]
        public string Text { get; set; } = null!;
    }

}
