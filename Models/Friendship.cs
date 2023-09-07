using AuthorVerseServer.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    [Index("User1"), Index("User2")]
    public class Friendship
    {
        [Key]
        public int FriendshipId { get; set; }
        public FriendshipStatus Status { get; set; }
        public User User1 { get; set; } = null!;
        public User User2 { get; set; } = null!;
    }
}
