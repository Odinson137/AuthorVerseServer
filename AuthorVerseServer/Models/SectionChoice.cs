
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class SectionChoice
    {
        [Key]
        public int ChoiceId { get; set; }
        public int ChoiceFlow { get; set; }
        public required string ChoiceText { get; set; }
        public int ContentId { get; set; }
        public int TargetSectionId { get; set; } // отправку на нужную главу в соответствии с выбором пользователя
        public ChapterSection TargetSection { get; set; }
    }
}
