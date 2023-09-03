namespace AuthorVerseServer.Models
{
    public class SectionChoice
    {
        public int SectionChoiceId { get; set; }
        public int ChoiceText { get; set; }
        public int TargetChapterId { get; set; } // отправку на нужную главу в соответствии с выбором пользователя
    }
}
