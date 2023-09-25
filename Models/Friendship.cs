using AuthorVerseServer.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Models
{
    [Index("User1Id"), Index("User2Id")]
    public class Friendship
    {
        public int User1Id { get; set; }
        public User User1 { get; set; } = null!;
        public int User2Id { get; set; }
        public User User2 { get; set; } = null!;
        public FriendshipStatus Status { get; set; }
    }
}
