namespace AuthorVerseServer.Models.ContentModels
{
    public class ChoiceContent : ContentBase
    {
        public ICollection<SectionChoice> SectionChoices { get; set; }
    }
}
