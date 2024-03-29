﻿using AsyncAwaitBestPractices;
using AuthorVerseServer.Data.ControllerSettings;
using AuthorVerseServer.Data.Patterns;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChapterSectionController : AuthorVerseController
    {
        private readonly IChapterSection _section;
        private readonly IDatabase _redis;

        public ChapterSectionController(IChapterSection chapterSection, IConnectionMultiplexer connectionMultiplexer)
        {
            _section = chapterSection;
            _redis = connectionMultiplexer.GetDatabase();
        }


        [HttpGet("GetAllWithModelContentBy")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<AllContentWithModelDTO>> GetAllWithModelContentSections(int chapterId, int flow, int lastChoiceNumber = 0)
        {
            var choiceContent = await _section.GetChoiceWithModelAsync(chapterId, flow, lastChoiceNumber);
            int choiceNumber = choiceContent?.Number ?? int.MaxValue;

            var contents = await _section.GetImmediatelyReadSectionsAsync(chapterId, flow, choiceNumber, lastChoiceNumber);

            var allContent = new AllContentWithModelDTO()
            {
                Choice = choiceContent,
                ContentsDTO = contents,
            };

            if (lastChoiceNumber != 0)
            {
                _redis.HashIncrementAsync($"choicer:{chapterId}",
                        $"flow-{flow}:number-{lastChoiceNumber}", 1, CommandFlags.FireAndForget)
                    .SafeFireAndForget();
            }
            
            return Ok(allContent);
        }
        
        [Authorize]
        [HttpGet("GetChoiceStatistic")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<int>> GetStatistic(int chapterId)
        {
            var result = await _redis.HashGetAllAsync($"choicer:{chapterId}");
            return Ok(result);
        }


        [HttpGet("GetAllContentBy")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<AllContentDTO>> GetAllContentSections(int chapterId, int flow, int lastChoiceNumber = 0)
        {
            var choiceContent = await _section.GetChoiceAsync(chapterId, flow, lastChoiceNumber);
            int choiceNumber = choiceContent?.Number ?? int.MaxValue;
            var contentIds = await _section.GetReadSectionsAsync(chapterId, flow, choiceNumber, lastChoiceNumber);

            var allContent = new AllContentDTO()
            {
                Choice = choiceContent,
                SectionsDTO = new List<SectionDTO>()
            };

            foreach (var content in contentIds)
            {
                var section = await UseContentType.GetContent(_section, content.ContentType).Invoke(content.ContentId);
                allContent.SectionsDTO.Add(section);
            }
            
            if (lastChoiceNumber != 0)
            {
                _redis.HashIncrementAsync($"choicer:{chapterId}",
                        $"flow-{flow}:number-{lastChoiceNumber}", 1, CommandFlags.FireAndForget)
                    .SafeFireAndForget();
            }
            
            return Ok(allContent);
        }
        


        [HttpGet("GetManagerBy")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ContentManagerDTO>> GetChapterSections(int chapterId, int flow, int lastChoiceNumber = 0)
        {
            var choiceContent = await _section.GetChoiceAsync(chapterId, flow, lastChoiceNumber);
            int choiceNumber = choiceContent?.Number ?? int.MaxValue;
            var contentIds = await _section.GetReadSectionsAsync(chapterId, flow, choiceNumber, lastChoiceNumber);

            ContentManagerDTO managerDTO = new ContentManagerDTO()
            {
                ContentsDTO = contentIds,
                Choice = choiceContent,
            };
            
            if (lastChoiceNumber != 0)
            {
                _redis.HashIncrementAsync($"choicer:{chapterId}",
                        $"flow-{flow}:number-{lastChoiceNumber}", 1, CommandFlags.FireAndForget)
                    .SafeFireAndForget();
            }

            return Ok(managerDTO);
        }

        [HttpGet("GetAutoTypeContentBy")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<SectionDTO>> GetAudioContentSections(int contentId, Data.Enums.ContentType type)
        {
            var section = await UseContentType.GetContent(_section, type).Invoke(contentId);
            return Ok(section);
        }

        [HttpGet("GetTextContentBy")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<SectionDTO>> GetTextContentSections(int contentId)
        {
            var section = await _section.GetTextContentAsync(contentId);
            return Ok(section);
        }

        [HttpGet("GetImageContentBy")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<SectionDTO>> GetImageContentSections(int contentId)
        {
            var section = await _section.GetImageContentAsync(contentId);
            return Ok(section);
        }

        [HttpGet("GetAudioContentBy")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<SectionDTO>> GetAudioContentSections(int contentId)
        {
            var section = await _section.GetAudioContentAsync(contentId);
            return Ok(section);
        }

        [HttpGet("GetVideoContentBy")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<SectionDTO>> GetVideoContentSections(int contentId)
        {
            var section = await _section.GetVideoContentAsync(contentId);
            return Ok(section);
        }
    }
}
