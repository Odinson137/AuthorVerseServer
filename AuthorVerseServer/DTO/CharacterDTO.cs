namespace AuthorVerseServer.DTO
{
    public class CharacterDTO
    {
        public required int CharacterId { get; set; }
        public required string Name { get; set; }
    }

    public class BookCharacterDTO
    {
        public required int CharacterId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? Url { get; set; }
    }
}
