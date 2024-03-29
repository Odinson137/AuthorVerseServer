﻿using AuthorVerseServer.Data.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace AuthorVerseServer.Models
{
    public class User : IdentityUser
    {
        public override string UserName { get; set; } = null!;
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? LogoUrl { get; set; }
        public string? Description { get; set; }
        public ICollection<ForumMessage> ForumMessages { get; set; } = new List<ForumMessage>();
        public ICollection<UserSelectedBook> UserSelectedBooks { get; set; } = new List<UserSelectedBook>();
        public ICollection<Book> Books { get; set; } = new List<Book>();
        public ICollection<Note> Notes { get; set; } = new List<Note>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<BookQuote> BookQuotes { get; set; } = new List<BookQuote>();
        public ICollection<Art> Arts { get; set; } = new List<Art>();
        public ICollection<Friendship> InitiatorFriendships { get; set; } = new List<Friendship>();
        public ICollection<Friendship> TargetFriendships { get; set; } = new List<Friendship>();
        public RegistrationMethod Method { get; set; } = RegistrationMethod.Email;
    }
}
