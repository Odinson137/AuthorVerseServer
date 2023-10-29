using AuthorVerseServer.Data.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthorVerseServer.Models
{
    public class User : IdentityUser
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? LogoUrl { get; set; }
        public string? Description { get; set; }
        public ICollection<UserSelectedBook> UserSelectedBooks { get; set; } = new List<UserSelectedBook>();
        public ICollection<Book> Books { get; set; } = new List<Book>();
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Friendship>? Friendships { get; set; }
        public ICollection<User>? Friends { get; set; }
        public RegistrationMethod Method { get; set; } = RegistrationMethod.Email;
    }
}
