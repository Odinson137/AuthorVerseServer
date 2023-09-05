using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthorVerseServer.Models
{
    public class User : IdentityUser
    {
        public Image? Logo { get; set; }
        public string? Description { get; set; }
        [InverseProperty("User")]
        public ICollection<UserBook>? UserBooks { get; set; }
    }
}
