﻿using AuthorVerseServer.Data.Patterns;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Interfaces.ServiceInterfaces;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChapterSectionController : ControllerBase
    {
        private readonly IChapterSection _section;
        private readonly IDatabase _redis;
        private readonly ISectionCreateManager _manager;

        private readonly CreateJWTtokenService _token;

        public ChapterSectionController(IChapterSection chapterSection, IConnectionMultiplexer redisConnection, ISectionCreateManager manager)
        {
            _section = chapterSection;
            _manager = manager;
            _redis = redisConnection.GetDatabase();
        }


        [HttpGet("GetAllContentBy")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<AllContentDTO>> GetAllContentSections(int chapterId, int flow)
        {
            var choiceContent = await _section.GetChoiceAsync(chapterId, flow, 0);
            int choiceNumber = choiceContent?.Number ?? int.MaxValue;
            var contentIds = await _section.GetReadSectionsAsync(chapterId, flow, choiceNumber, 0);

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

            return Ok(allContent);
        }

        [HttpGet("GetManagerBy/{chapterId}/{flow}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ContentManagerDTO>> GetChapterSections(int chapterId, int flow)
        {
            var choiceContent = await _section.GetChoiceAsync(chapterId, flow, 0);
            int choiceNumber = choiceContent?.Number ?? int.MaxValue;
            var contentIds = await _section.GetReadSectionsAsync(chapterId, flow, choiceNumber, 0);

            ContentManagerDTO mangerDTO = new ContentManagerDTO()
            {
                ContentsDTO = contentIds,
                Choice = choiceContent,
            };

            return Ok(mangerDTO);
        }

        [HttpGet("GetManagerBy/{chapterId}/{flow}/{lastChoiceNumber}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ContentManagerDTO>> GetChapterSections(int chapterId, int flow, int lastChoiceNumber)
        {
            var choiceContent = await _section.GetChoiceAsync(chapterId, flow, lastChoiceNumber);
            int choiceNumber = choiceContent?.Number ?? int.MaxValue;
            var contentIds = await _section.GetReadSectionsAsync(chapterId, flow, choiceNumber, lastChoiceNumber);

            ContentManagerDTO managerDTO = new ContentManagerDTO()
            {
                ContentsDTO = contentIds,
                Choice = choiceContent,
            };

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

        /// <summary>
        /// Registers the user in the chapter creation manager
        /// </summary>
        /// <param name="chapterId"></param>
        /// <returns>if user already had in process to create the chapter, return CreateManagerDTO else nothing</returns>
        [Authorize]
        [HttpPost("CreateManager")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<SortedSetEntry[]?>> CreateBookManager(int chapterId)
        {
            string? userId = _token.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Token user is not correct");

            return Ok(await _manager.CreateManagerAsync(userId, chapterId));
        }

        /// <summary>
        /// It should check whether the section with the number and the flow exist in db or contained in redis
        /// And if section is contained on redis, then replace it with new object
        /// </summary>
        /// <param name="number">the chapter section number</param>
        /// <param name="flow">the flow in which user create a new section</param>
        /// <param name="text"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("CreateTextSection")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateNewTextSection(int number, int flow, string text)
        {
            var userId = _token.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Token user is not correct");

            var error = await _manager.CreateTextSectionAsync(userId, number, flow, text);
            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest(error);
            }

            return Ok();
        }
    }
}
