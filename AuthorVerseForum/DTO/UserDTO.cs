namespace AuthorVerseForum.DTO
{
    public class UserVerify
    {
        public required string Id { get; set; }
        public required string UserName { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public string? Logo { get; set; }
    }
}
