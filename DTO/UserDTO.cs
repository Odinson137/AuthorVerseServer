namespace AuthorVerseServer.DTO
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
    }

    public class UserLoginDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class UserRegistrationDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }

    public class UserVerify
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Token { get; set; } = null!;
    }

    public class UserGoogleVerify
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string? IconUrl { get; set; }
        public string Token { get; set; } = null!;
    }

    public class UserMicrosoftVerify
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Token { get; set; } = null!;
    }

    public class UserProfile
    {
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string UserPrincipalName { get; set; }
        public string Id { get; set; }
    }
}
