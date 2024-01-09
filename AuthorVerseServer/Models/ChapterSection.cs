using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.Models.ContentModels;

namespace AuthorVerseServer.Models
{
    public class ChapterSection
    {
        public int BookChapterId { get; set; }
        public BookChapter BookChapter { get; set; } = null!;
        public required int Number { get; set; }
        public required int ChoiceFlow { get; set; }
        public required ContentType ContentType { get; set; }
        public int ContentId { get; set; }
        public required ContentBase ContentBase { get; set; }
        public bool Visibility { get; set; } = true;
        public ICollection<SectionChoice> SectionChoices { get; set; } = new List<SectionChoice>();
    }
}
