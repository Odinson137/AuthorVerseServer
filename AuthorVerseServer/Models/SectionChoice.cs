
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class SectionChoice
    {
        [Key]
        public int ChoiceId { get; set; }
        public int ChapterSectionId { get; set; }
        public ChapterSection ChapterSection { get; set; } = null!;
        public required string ChoiceText { get; set; }
        public int TargetSectionId { get; set; } // отправку на нужную главу в соответствии с выбором пользователя
        public ChapterSection TargetSection { get; set; } = null!;
    }
}
