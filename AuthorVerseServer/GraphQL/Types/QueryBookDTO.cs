using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.Models;
using Tag = AuthorVerseServer.Models.Tag;

namespace AuthorVerseServer.GraphQL.Types;

public class QueryBookDTO
{
    public int BookId { get; set; }
    public string Title { get; set; }
    public string NormalizedTitle { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string AuthorId { get; set; } = null!;
    public string? AuthorUserName { get; set; }
    public string? AuthorName { get; set; }
    public string? AuthorLogoUrl { get; set; }
    public string? AuthorDescription { get; set; }
    public string? AuthorLastName { get; set; }
    public string? AuthorEmail { get; set; }
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public ICollection<Art> Arts { get; set; } = new List<Art>();
    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Character> Characters { get; set; } = new List<Character>();
    public ICollection<BookChapter> BookChapters { get; set; } = new List<BookChapter>();
    public ICollection<ForumMessage> ForumMessages { get; set; } = new List<ForumMessage>();
    public ICollection<UserSelectedBook> UserSelectedBooks { get; set; } = new List<UserSelectedBook>();
    public ICollection<BookQuote> BookQuotes { get; set; } = new List<BookQuote>();
    public DateTime PublicationData { get; set; } = DateTime.Now; 
    public AgeRating AgeRating { get; set; }
    public string? BookCover { get; set; }
    public double Rating { get; set; }
    public int CountRating { get; set; }
    public double Earnings { get; set; }
    public PublicationPermission Permission { get; set; } = PublicationPermission.Approved;
}

public class CreateBookGraphDTO
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public ICollection<int>? GenresId { get; set; }
    public ICollection<int>? TagsId { get; set; }
    public AgeRating AgeRating { get; set; }
}

public class UpdateBookGraphDTO
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public ICollection<int>? GenresId { get; set; }
    public ICollection<int>? TagsId { get; set; }
    public AgeRating AgeRating { get; set; }
}