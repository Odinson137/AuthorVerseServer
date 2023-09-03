namespace AuthorVerseServer.Models
{
    public class ChapterSection
    {
        public int SectionId { get; set; }
        public int Number { get; set; }
        public string? Text { get; set; } // либо это
        public Image? Image { get; set; } // либо это
        public int NextSectionId { get; set; }
        public ICollection<SectionChoice>? SectionChoices { get; set; }
    }
}
