using AuthorVerseServer.DTO;

namespace AuthorVerseServer.Interfaces
{
    public interface IChapterSection
    {
        public Task<bool> CheckAddingNewSectionAsync(int number, int flow);
        public Task<ChoiceBaseDTO?> GetChoiceAsync(int chapterId, int flow, int lastChoiceNumber);
        public Task<List<ContentDTO>> GetReadSectionsAsync(int chapterId, int choiceFlow, int choiceNumber, int lastChoiceNumber = 0);
        public Task<SectionDTO> GetTextContentAsync(int contentId);
        public Task<SectionDTO> GetAudioContentAsync(int contentId);
        public Task<SectionDTO> GetVideoContentAsync(int contentId);
        public Task<SectionDTO> GetImageContentAsync(int contentId);
    }
}
