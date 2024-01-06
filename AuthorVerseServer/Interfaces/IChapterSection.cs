using AuthorVerseServer.Data.JsonModels;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;
using AuthorVerseServer.Models.ContentModels;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AuthorVerseServer.Interfaces
{
    public interface IChapterSection
    {
        public Task<TransferInfoDTO> GetTransferInfoAsync(int bookId);
        public Task<int> ChangeVisibilityAsync(int chapterId, int number, int flow, bool newValue);
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
    }
}
