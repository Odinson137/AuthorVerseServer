namespace AuthorVerseServer.Models
{
    public class CharacterChapter
    {
        public int CharacterId { get; set; }
        public Character Character { get; set; } = null!;
        public int ChapterId { get; set; }
        public BookChapter Chapter { get; set; } = null!;
    }
}
