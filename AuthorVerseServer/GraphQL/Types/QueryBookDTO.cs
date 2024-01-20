using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.Models;
using Tag = AuthorVerseServer.Models.Tag;

namespace AuthorVerseServer.GraphQL.Types;

public class QueryBookDTOJust
{
    public int BookId { get; set; }
    public required string Title { get; set; }
    public string NormalizedTitle { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string AuthorId { get; set; } = null!;
    public QueryUserDTO Author { get; set; } = null!;
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

public class QueryBookDTO
{
    public QueryBookDTO()
    {
        
    }
    
    public QueryBookDTO(Book c)
    {
        BookId = c.BookId;
        Title = c.Title;
        Description = c.Description;
        Tags = c.Tags;
        Genres = c.Genres;
        Comments = c.Comments;
        Arts = c.Arts;
        Characters = c.Characters;
        BookChapters = c.BookChapters;
        ForumMessages = c.ForumMessages;
        UserSelectedBooks = c.UserSelectedBooks;
        BookCover = c.BookCover;
        BookQuotes = c.BookQuotes;
        Permission = c.Permission;
        NormalizedTitle = c.NormalizedTitle;
        AuthorId = c.AuthorId;
        Rating = c.Rating;
        AgeRating = c.AgeRating;
        Earnings = c.Earnings;
        CountRating = c.CountRating;
        PublicationData = c.PublicationData;
    }
    public int BookId { get; set; }
    public string Title { get; set; }
    public string NormalizedTitle { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string AuthorId { get; set; } = null!;
    public QueryUserDTO Author { get; set; } = null!;
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