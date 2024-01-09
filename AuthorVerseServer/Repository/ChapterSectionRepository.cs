using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Repository
{
    public class ChapterSectionRepository : IChapterSection
    {
        private readonly DataContext _context;
        public ChapterSectionRepository(DataContext context)
        {
            _context = context;
        }

        public Task<TransferInfoDTO> GetTransferInfoAsync(int bookId)
        {
            var uniqueValues = _context.ChapterSections
                .Where(c => c.BookChapter.BookId == bookId)
                .GroupBy(c => 1) 
                .Select(group => new TransferInfoDTO
                {
                    Chapters = group.Select(c => c.BookChapterId).Distinct().ToList(),
                    Numbers = group.Select(c => c.Number).Distinct().ToList(),
                    Flows = group.Select(c => c.ChoiceFlow).Distinct().ToList()
                });

            return uniqueValues.FirstAsync();
        }

        public Task<int> ChangeVisibilityAsync(int chapterId, int number, int flow, bool newValue)
        {
            return _context.ChapterSections
                .Where(c => c.BookChapterId == chapterId)
                .Where(c => c.Number == number)
                .Where(c => c.ChoiceFlow == flow)
                .ExecuteUpdateAsync(setter => setter.SetProperty(
                    x => x.Visibility, newValue));
        }


        public Task<ChoiceBaseWithModelDTO?> GetChoiceWithModelAsync(int chapterId, int flow, int lastChoiceNumber)
        {
            var choiceContent = _context.ChapterSections
                .OrderBy(c => c.Number)
                .Where(c => c.BookChapterId == chapterId)
                .Where(c => c.Number > lastChoiceNumber)
                .Where(c => c.ChoiceFlow == flow)
                .Where(c => c.Visibility == true)
                .Where(c => c.SectionChoices != null && c.SectionChoices.Count() >= 1)
                .Include(c => c.ContentBase)
                .Select(c => new ChoiceBaseWithModelDTO
                {
                    ChoiceFlow = c.ChoiceFlow,
                    Number = c.Number,
                    Content = c.ContentBase,
                    ContentType = c.ContentType,
                    SectionChoices = c.SectionChoices!.Select(sc => new SectionChoiceDTO
                    {
                        ChoiceNumber = sc.ChoiceNumber,
                        ChoiceFlow = sc.TargetSection.ChoiceFlow,
                        Number = sc.TargetSection.Number,
                        ChoiceText = sc.ChoiceText,
                    }).ToList(),
                });

            return choiceContent.FirstOrDefaultAsync();
        }

        public Task<ChoiceBaseDTO?> GetChoiceAsync(int chapterId, int flow, int lastChoiceNumber)
        {
            var choiceContent = _context.ChapterSections
                .OrderBy(c => c.Number)
                .Where(c => c.BookChapterId == chapterId)
                .Where(c => c.Number > lastChoiceNumber)
                .Where(c => c.ChoiceFlow == flow)
                .Where(c => c.Visibility == true)
                .Where(c => c.SectionChoices != null && c.SectionChoices.Count() >= 1)
                .Select(c => new ChoiceBaseDTO
                {
                    ChoiceFlow = c.ChoiceFlow,
                    Number = c.Number,
                    ContentId = c.ContentId,
                    ContentType = c.ContentType,
                    SectionChoices = c.SectionChoices!.Select(sc => new SectionChoiceDTO
                    {
                        ChoiceNumber = sc.ChoiceNumber,
                        ChoiceFlow = sc.TargetSection.ChoiceFlow,
                        Number = sc.TargetSection.Number,
                        ChoiceText = sc.ChoiceText,
                    }).ToList(),
                });

            return choiceContent.FirstOrDefaultAsync();
        }

        public Task<List<ContentWithModelDTO>> GetImmediatelyReadSectionsAsync(int chapterId, int choiceFlow, int choiceNumber, int lastChoiceNumber = 0)
        {
            var choiceContent = _context.ChapterSections
                .Include(c => c.ContentBase)
                .OrderBy(c => c.Number)
                .Where(c => c.BookChapterId == chapterId)
                .Where(c => c.ChoiceFlow == choiceFlow)
                .Where(c => c.Visibility == true)
                .Where(c => c.Number < choiceNumber)
                .Where(c => c.Number > lastChoiceNumber)
                .Select(c => new ContentWithModelDTO
                {
                    ChoiceFlow = c.ChoiceFlow,
                    Number = c.Number,
                    ContentType = c.ContentType,
                    Content = c.ContentBase,
                });

            return choiceContent.ToListAsync();
        }

        public Task<List<ContentDTO>> GetReadSectionsAsync(int chapterId, int choiceFlow, int choiceNumber, int lastChoiceNumber = 0)
        {
            var choiceContent = _context.ChapterSections
                .OrderBy(c => c.Number)
                .Where(c => c.BookChapterId == chapterId)
                .Where(c => c.ChoiceFlow == choiceFlow)
                .Where(c => c.Number < choiceNumber)
                .Where(c => c.Number > lastChoiceNumber)
                .Where(c => c.Visibility == true)
                .Select(c => new ContentDTO
                {
                    ChoiceFlow = c.ChoiceFlow,
                    Number = c.Number,
                    ContentId = c.ContentId,
                    ContentType = c.ContentType,
                });

            return choiceContent.ToListAsync();
        }


        public Task<SectionDTO> GetTextContentAsync(int contentId)
        {
            var section = _context.TextContents
                .Where(c => c.ContentId == contentId)
                .Select(c => new SectionDTO()
                {
                    Content = c.Text,
                    Type = Data.Enums.ContentType.Text,
                });

            return section.SingleAsync();
        }


        public Task<SectionDTO> GetAudioContentAsync(int contentId)
        {
            var section = _context.AudioContents
                .Where(c => c.ContentId == contentId)
                .Select(c => new SectionDTO()
                {
                    Content = c.Url,
                    Type = Data.Enums.ContentType.Audio,
                });

            return section.SingleAsync();
        }

        public Task<SectionDTO> GetVideoContentAsync(int contentId)
        {
            var section = _context.VideoContents
                .Where(c => c.ContentId == contentId)
                .Select(c => new SectionDTO()
                {
                    Content = c.Url,
                    Type = Data.Enums.ContentType.Video,
                });

            return section.SingleAsync();
        }

        public Task<SectionDTO> GetImageContentAsync(int contentId)
        {
            var section = _context.ImageContents
                .Where(c => c.ContentId == contentId)
                .Select(c => new SectionDTO()
                {
                    Content = c.Url,
                    Type = Data.Enums.ContentType.Image,
                });

            return section.SingleAsync();
        }
    }
}
