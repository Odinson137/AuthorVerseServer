using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models;

public class Art
{
    public Art(string url, User user, Book book)
    {
        Url = url;
        User = user;
        Book = book;
    }

    public Art()
    {
        
    }
    public int ArtId { get; set; }
    [MaxLength(50)]
    public string Url { get; set; }
    public string UserId { get; set; }
    public User User { get; set; } = null!;
    public int BookId { get; set; }
    public Book Book { get; set; }
    public DateTime CreateDateTime { get; set; } = DateTime.Now;
}