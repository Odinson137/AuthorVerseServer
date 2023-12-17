using AuthorVerseServer.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.DTO
{
    public class CommentDTO
    {
        public required int BaseId { get; set; }
        public required UserViewDTO User { get; set; }
        public required string Text { get; set; }
        [Range(1, 5)]
        public required int ReaderRating { get; set; }
        public LikeRating IsRated { get; set; }
        public required int Likes { get; set; }
        public required  int DisLikes { get; set; }
        public required DateOnly CreatedDateTime { get; set; }
    }


    public class CreateCommentDTO
    {
        [Required]
        public int BookId { get; set; }
        [Required]
        [MaxLength(400, ErrorMessage = "Messege's too big")]
        [MinLength(50, ErrorMessage = "Messege's too short")]
        public string Text { get; set; } = null!;
        [Range(1, 5)]
        public int Rating { get; set; }
    }

}
