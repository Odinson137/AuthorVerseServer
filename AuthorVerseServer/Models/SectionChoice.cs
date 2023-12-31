
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class SectionChoice
    {
        [Key]
        public int ChoiceId { get; set; }
        //public int ChoiceFlow { get; set; }
        //public int Number { get; set; }
        public int ChapterSectionId { get; set; }
        public ChapterSection ChapterSection { get; set; } = null!;
        public required string ChoiceText { get; set; }
        //public int TargetChoice { get; set; } // отправку на нужную главу в соответствии с выбором пользователя
        //public int TargetNumber { get; set; }
        public int TargetSectionId { get; set; }
        public ChapterSection TargetSection { get; set; } = null!;
    }
}
