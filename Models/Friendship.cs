using AuthorVerseServer.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Models
{
    [Index("User1"), Index("User2")]
    public class Friendship
    {
        public User User1 { get; set; } = null!;
        public User User2 { get; set; } = null!;
        public FriendshipStatus Status { get; set; }
    }
}
