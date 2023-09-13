using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthorVerseServer.Models
{
    
    public class User : IdentityUser
    {
        public Image? Logo { get; set; }
        public string? Description { get; set; }
        public ICollection<UserSelectedBook> UserSelectedBook { get; set; } = new List<UserSelectedBook>();
        public ICollection<Book> Books { get; set; } = new List<Book>();
        public ICollection<Comment>? Comments { get; set; }
        [NotMapped]
        public ICollection<Friendship>? Friendships { get; set; }
        public ICollection<User>? Friends { get; set; }
    }
}
