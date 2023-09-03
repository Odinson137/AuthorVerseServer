﻿using AuthorVerseServer.Enums;

namespace AuthorVerseServer.Models
{
    public class Friendship
    {
        public int FriendshipId { get; set; }
        public FriendshipStatus Status { get; set; }
        public User User1 { get; set; } = null!;
        public User User2 { get; set; } = null!;
    }
}