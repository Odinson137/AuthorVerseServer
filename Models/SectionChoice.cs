using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthorVerseServer.Models
{
    public class SectionChoice
    {
        [Key]
        public int SectionChoiceId { get; set; }

        public int ChapterSectionId { get; set; }
        [ForeignKey("ChapterSectionId")]
        public ChapterSection ChapterSection { get; set; } = null!;
        public int TargetSectionId { get; set; } // отправку на нужную главу в соответствии с выбором пользователя
        public int ChoiceText { get; set; }
    }
}
