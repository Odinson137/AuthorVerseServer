namespace AuthorVerseForum.DTO
{
    public class UserVerify
    {
        public required string UserName { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public string? Logo { get; set; }
    }

    public class UserMessageDTO
    {
        public UserMessageDTO(UserVerify user, string userId)
        {
            if (string.IsNullOrEmpty(user.Name))
                UserName = user.UserName;
            else
                UserName = $"{user.Name} {user.LastName}";

            Id = userId;
            Logo = user.Logo;
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string? Logo { get; set; }
    }
}
