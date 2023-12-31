using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.Models.ContentModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthorVerseServer.Models
{
    public class ChapterSection
    {
        [Key]
        public int SectionId { get ; set; }
        public int BookChapterId { get; set; }
        public BookChapter BookChapter { get; set; } = null!;
        public required int Number { get; set; }
        public required int ChoiceFlow { get; set; }
        public required ContentType ContentType { get; set; }
        public int ContentId { get; set; }
        public required ContentBase ContentBase { get; set; }
        public bool Visibility { get; set; } = true;
        public ICollection<SectionChoice>? SectionChoices { get; set; }
    }
}
