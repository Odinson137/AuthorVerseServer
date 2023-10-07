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
}
