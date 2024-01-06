
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class SectionChoice
    {
        // [Key]
        // public int ChoiceId { get; set; }
        public required int ChoiceNumber { get; set; }
        public int ChapterId { get; set; }
        public int Number { get; set; }
        public int Flow { get; set; }
        public ChapterSection ChapterSection { get; set; } = null!;
        public required string ChoiceText { get; set; }
        public int TargetChapterId { get; set; }
        public int TargetNumber { get; set; }
        public int TargetFlow { get; set; }
        public ChapterSection TargetSection { get; set; } = null!;
    }
}
