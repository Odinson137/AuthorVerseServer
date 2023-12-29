using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models.ContentModels;
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

        /// <summary>
        /// Check a new section can be added to the db
        /// </summary>
        /// <param name="chapterId"></param>
        /// <param name="flow">The flow that describe current user choice</param>
        /// <returns></returns>
        public Task<int> CheckAddingNewSectionAsync(int chapterId, int flow)
        {
            var value = _context.ChapterSections
                .Where(c => c.BookChapterId == chapterId)
                .Where(c => c.ChoiceFlow == flow)
                .MaxAsync(c => c.Number);
            return value;
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
                        ChoiceFlow = sc.TargetSection.ChoiceFlow,
                        Number = sc.TargetSection.Number,
                        ChoiceText = sc.ChoiceText,
                    }).ToList(),
                });

            return choiceContent.FirstOrDefaultAsync();
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

        public Task AddContentAsync(Models.ContentModels.TextContent content)
        {
            throw new NotImplementedException();
        }

        public Task AddContentAsync(Models.ContentModels.ImageContent content)
        {
            throw new NotImplementedException();
        }

        public Task AddContentAsync(VideoContent content)
        {
            throw new NotImplementedException();
        }

        public Task AddContentAsync(AudioContent content)
        {
            throw new NotImplementedException();
        }
    }
}
