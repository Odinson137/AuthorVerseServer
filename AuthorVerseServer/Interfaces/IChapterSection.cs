using AuthorVerseServer.DTO;
using AuthorVerseServer.Models.ContentModels;

namespace AuthorVerseServer.Interfaces
{
    public interface IChapterSection
    {
        public Task<int> CheckAddingNewSectionAsync(int chapterId, int flow);
        public Task<ChoiceBaseDTO?> GetChoiceAsync(int chapterId, int flow, int lastChoiceNumber);
        public Task<List<ContentDTO>> GetReadSectionsAsync(int chapterId, int choiceFlow, int choiceNumber, int lastChoiceNumber = 0);
        public Task<SectionDTO> GetTextContentAsync(int contentId);
        public Task<SectionDTO> GetAudioContentAsync(int contentId);
        public Task<SectionDTO> GetVideoContentAsync(int contentId);
        public Task<SectionDTO> GetImageContentAsync(int contentId);
        public Task AddContentAsync(Models.ContentModels.TextContent content);
        public Task AddContentAsync(Models.ContentModels.ImageContent content);
        public Task AddContentAsync(Models.ContentModels.VideoContent content);
        public Task AddContentAsync(Models.ContentModels.AudioContent content);
    }
}
