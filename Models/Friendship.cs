using AuthorVerseServer.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthorVerseServer.Models
{
    public class Friendship
    {
        public string User1Id { get; set; }
        public User User1 { get; set; } = null!;
        public string User2Id { get; set; }
        public User User2 { get; set; } = null!;
        public FriendshipStatus Status { get; set; } = FriendshipStatus.Pending;
    }
}
