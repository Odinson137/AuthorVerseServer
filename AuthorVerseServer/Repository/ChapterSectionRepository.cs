﻿using AuthorVerseServer.Data;
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

        public async Task<ChoiceBaseDTO?> GetChoiceAsync(int chapterId, int flow, int lastChoiceNumber)
        {
            var choiceContent = _context.ChapterSections
                .OrderBy(c => c.Number)
                .Where(c => c.BookChapterId == chapterId)
                .Where(c => c.Number > lastChoiceNumber)
                .Where(c => c.ChoiceFlow == flow)
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

            return await choiceContent.FirstOrDefaultAsync();
        }


        public async Task<ICollection<ContentDTO>> GetReadSectionsAsync(int chapterId, int choiceFlow, int choiceNumber, int lastChoiceNumber = 0)
        {
            var choiceContent = _context.ChapterSections
                .OrderBy(c => c.Number)
                .Where(c => c.BookChapterId == chapterId)
                .Where(c => c.ChoiceFlow == choiceFlow)
                .Where(c => c.Number < choiceNumber)
                .Where(c => c.Number > lastChoiceNumber)
                .Select(c => new ContentDTO
                {
                    ChoiceFlow = c.ChoiceFlow,
                    Number = c.Number,
                    ContentId = c.ContentId,
                    ContentType = c.ContentType,
                });

            return await choiceContent.ToListAsync();
        }


        public async Task<SectionDTO> GetTextContentAsync(int contentId)
        {
            var section = _context.TextContents
                .Where(c => c.ContentId == contentId)
                .Select(c => new SectionDTO()
                {
                    Content = c.Text,
                    Type = Data.Enums.ContentType.Text,
                });

            return await section.SingleAsync();
        }


        public async Task<SectionDTO> GetAudioContentAsync(int contentId)
        {
            var section = _context.AudioContents
                .Where(c => c.ContentId == contentId)
                .Select(c => new SectionDTO()
                {
                    Content = c.Url,
                    Type = Data.Enums.ContentType.Audio,
                });

            return await section.SingleAsync();
        }

        public async Task<SectionDTO> GetVideoContentAsync(int contentId)
        {
            var section = _context.VideoContents
                .Where(c => c.ContentId == contentId)
                .Select(c => new SectionDTO()
                {
                    Content = c.Url,
                    Type = Data.Enums.ContentType.Video,
                });

            return await section.SingleAsync();
        }

        public async Task<SectionDTO> GetImageContentAsync(int contentId)
        {
            var section = _context.ImageContents
                .Where(c => c.ContentId == contentId)
                .Select(c => new SectionDTO()
                {
                    Content = c.Url,
                    Type = Data.Enums.ContentType.Image,
                });

            return await section.SingleAsync();
        }
    }
}
