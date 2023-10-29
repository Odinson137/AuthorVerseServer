using AuthorVerseServer.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthorVerseServer.Models
{
    [Index("User1Id"), Index("User2Id")]
    public class Friendship
    {
        public string User1Id { get; set; }
        //[ForeignKey("User1Id")]
        public User User1 { get; set; } = null!;
        public string User2Id { get; set; }
        //[ForeignKey("User2Id")]
        public User User2 { get; set; } = null!;
        public FriendshipStatus Status { get; set; }
    }
}
