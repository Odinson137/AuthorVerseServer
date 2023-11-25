using AuthorVerseServer.Data.Enums;

namespace AuthorVerseServer.DTO
{
    public class UserProfileDTO
    {
        public string UserName { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
    }
    public class UpdateAccountBook
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; } = null!;
        public int ChapterNumber { get; set; }
        public string? BookCoverUrl { get; set; }
    }

    public class SelectedUserBookDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public BookState BookState { get; set; }
        public DateOnly PublicationData { get; set; }
        public int LastReadingChapter { get; set; }
        public int LastBookChapter { get; set; }
    }

    public class UserBookDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? BookCoverUrl { get; set; }
        public double Earnings { get; set; }
        public DateOnly PublicationData { get; set; }
    }


    public class CommentPageDTO
    {
        public int PagesCount { get; set; }
        public ICollection<CommentProfileDTO> comments = new List<CommentProfileDTO>();
    }


    public class CommentProfileDTO
    {
        public int CommentId { get; set; }
        public string Text { get; set; } = null!;
        public int Rating { get; set; }
        public int Likes { get; set; } = 0;
        public int DisLikes { get; set; } = 0;
        public CommentType CommentType { get; set; }
        public string BookTitle { get; set; } = null!;
        public int ChapterNumber { get; set; }
        public string? ChapterTitle { get; set; }
        public DateOnly CommentCreatedDateTime { get; set; }
    }

    public class FriendDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public DateOnly FriendShipTime { get; set; }
        public FriendshipStatus Status { get; set; }
    }
}
