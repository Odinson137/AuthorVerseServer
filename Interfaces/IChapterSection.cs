using AuthorVerseServer.Models;

namespace AuthorVerseServer.Interfaces
{
    public interface IChapterSection
    {
        Task<ICollection<ChapterSection>> GetChapterSectionAsync();
    }
}
