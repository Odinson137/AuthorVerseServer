using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;
using AuthorVerseServer.Models.ContentModels;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AuthorVerseServer.Interfaces
{
    public interface IChapterSection
    {
        public Task<int> SaveAsync();
        public void DeleteContent(ContentBase content);
        public void DeleteSection(ChapterSection chapter);
        public Task<ChapterSection> GetSectionAsync(int chapterId, int number, int flow);
        public Task<int> CheckAddingNewSectionAsync(int chapterId, int flow);
        public Task<ChoiceBaseWithModelDTO?> GetChoiceWithModelAsync(int chapterId, int flow, int lastChoiceNumber);
        public Task<ChoiceBaseDTO?> GetChoiceAsync(int chapterId, int flow, int lastChoiceNumber);

        public Task<List<ContentWithModelDTO>> GetImmediatelyReadSectionsAsync(int chapterId, int choiceFlow,
            int choiceNumber, int lastChoiceNumber = 0);

        public Task<List<ContentDTO>> GetReadSectionsAsync(int chapterId, int choiceFlow, int choiceNumber,
            int lastChoiceNumber = 0);
        public Task<SectionDTO> GetTextContentAsync(int contentId);
        public Task<SectionDTO> GetAudioContentAsync(int contentId);
        public Task<SectionDTO> GetVideoContentAsync(int contentId);
        public Task<SectionDTO> GetImageContentAsync(int contentId);
        public ValueTask<EntityEntry> AddContentAsync<T>(T content);
    }
}
