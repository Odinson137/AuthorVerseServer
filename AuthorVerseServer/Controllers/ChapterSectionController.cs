﻿using AuthorVerseServer.Data.ControllerSettings;
using AuthorVerseServer.Data.Patterns;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChapterSectionController : AuthorVerseController
    {
        private readonly IChapterSection _section;
        private readonly ISectionCreateManager _manager;

        public ChapterSectionController(IChapterSection chapterSection, 
            ISectionCreateManager manager)
        {
            _section = chapterSection;
            _manager = manager;
        }


        [HttpGet("GetAllWithModelContentBy")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<AllContentWithModelDTO>> GetAllWithModelContentSections(int chapterId, int flow)
        {
            var choiceContent = await _section.GetChoiceWithModelAsync(chapterId, flow, 0);
            int choiceNumber = choiceContent?.Number ?? int.MaxValue;

            var contents = await _section.GetImmediatelyReadSectionsAsync(chapterId, flow, choiceNumber, 0);

            var allContent = new AllContentWithModelDTO()
            {
                Choice = choiceContent,
                ContentsDTO = contents,
            };

            return Ok(allContent);
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
        /// <returns>if user already had in process to create the chapter, return CreateManagerDTO else nothing</returns>
        [Authorize]
        [HttpPost("CreateManager/{chapterId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ICollection<string>?>> CreateBookManager(int chapterId)
        {
            var value = await _manager.CreateManagerAsync(UserId, chapterId);
            return Ok(value);
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
        [ProducesResponseType(201)]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<ActionResult> CreateNewTextSection(int number, int flow, string text)
        {
            try
            {
                await _manager.AddSectionToRedisAsync(UserId, number, flow, "text", text);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [Authorize]
        [HttpPost("CreateImageSection")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<ActionResult> CreateNewImageSection(int number, int flow, IFormFile imageFile)
        {
            try
            {
                await _manager.AddSectionToRedisAsync(UserId, number, flow, "image", imageFile);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [Authorize]
        [HttpPost("CreateAudioSection")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<ActionResult> CreateNewAudioSection(int number, int flow, IFormFile file)
        {
            try
            {
                await _manager.AddSectionToRedisAsync(UserId, number, flow, "audio", file);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }
        
        [Authorize]
        [HttpPut("UpdateTextSection")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<ActionResult> UpdateImageSection(int number, int flow, string text)
        {
            try
            {
                await _manager.UpdateSectionToRedisAsync(UserId, number, flow, "text", text);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [Authorize]
        [HttpPut("UpdateVideoSection")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<ActionResult> UpdateVideoSection(int number, int flow, IFormFile formFile)
        {
            try
            {
                await _manager.UpdateSectionToRedisAsync(UserId, number, flow, "image", formFile);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [Authorize]
        [HttpPut("UpdateAudioSection")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<ActionResult> UpdateAudioSection(int number, int flow, IFormFile formFile)
        {
            try
            {
                await _manager.UpdateSectionToRedisAsync(UserId, number, flow, "audio", formFile);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }
        
        [Authorize]
        [HttpDelete("DeleteSection")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<ActionResult> DeleteSection(int number, int flow)
        {
            try
            {
                await _manager.DeleteSectionFromRedisAsync(UserId, number, flow);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [Authorize]
        [HttpPost("FinallySave")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<ActionResult> SaveSectionFromManager()
        {
            try
            {
                await _manager.ManagerSaveAsync(UserId);
            }
            catch (Exception e)
            {
                BadRequest(e.Message);
            }

            return Ok();
        }
    }
}
