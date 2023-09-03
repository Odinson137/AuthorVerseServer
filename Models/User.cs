using AuthorVerseServer.Enums;
using Microsoft.AspNetCore.Identity;

namespace AuthorVerseServer.Models
{
    public class User : IdentityUser
    {
        public Image? Logo { get; set; }
        public string? Description { get; set; }
        public ICollection<UserBook>? UserBooks { get; set; }
    }
}
