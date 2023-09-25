using AuthorVerseServer.Models;

namespace AuthorVerseServer.Interfaces
{
    public interface ISectionChoice
    {
        Task<ICollection<SectionChoice>> GetSectionChoiceAsync();
    }
}
