using AuthorVerseServer.Data.ControllerSettings;
using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.Data.JsonModels;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Interfaces.ServiceInterfaces;
using AuthorVerseServer.Interfaces.ServiceInterfaces.SectionCreateManager;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace AuthorVerseServer.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CreatorController : AuthorVerseController
{
    private readonly IChapterSection _section;
    private readonly ISectionCreateManager _manager;
    private readonly IDatabase _redis;
    private readonly ICudChoiceOperations _choiceService;
    private readonly ICudOperations _textService;
    private readonly ICudOperations _imageService;
    private readonly ICudOperations _audioService;
    private readonly BaseCudService _baseService;

    public CreatorController(IChapterSection chapterSection,
        ISectionCreateManager manager, IConnectionMultiplexer connectionMultiplexer,
        ICudChoiceOperations choiceService,
        [FromKeyedServices("text")] ICudOperations textService, 
        [FromKeyedServices("image")] ICudOperations imageService, 
        [FromKeyedServices("audio")] ICudOperations audioService, 
        BaseCudService baseService)
        {
            _section = chapterSection;
            _manager = manager;
            _choiceService = choiceService;
            _textService = textService;
            _imageService = imageService;
            _audioService = audioService;
            _baseService = baseService;
            _redis = connectionMultiplexer.GetDatabase();
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
                await _textService.CreateSectionAsync(UserId, number, flow, text);
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
                await _imageService.CreateSectionAsync(UserId, number, flow, imageFile);
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
                await _audioService.CreateSectionAsync(UserId, number, flow, file);
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
                await _textService.UpdateSectionAsync(UserId, number, flow, text);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [Authorize]
        [HttpPut("UpdateImageSection")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<ActionResult> UpdateVideoSection(int number, int flow, IFormFile formFile)
        {
            try
            {
                await _imageService.UpdateSectionAsync(UserId, number, flow, formFile);
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
                await _audioService.UpdateSectionAsync(UserId, number, flow, formFile);
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
                await _baseService.DeleteSectionFromRedisAsync(UserId, number, flow);
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
        
        [Authorize]
        [HttpPatch("ChangeVisibility")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<ActionResult> ChangeVisibility(int number, int flow, bool newValue)
        {
            var chapterId = await _redis.StringGetAsync($"managerInfo:{UserId}");
            if (chapterId.IsNullOrEmpty)
            {
                return BadRequest("Session has time out");
            }
            
            await _section.ChangeVisibilityAsync((int)chapterId, number, flow, newValue);
            return Ok();
        }
        
        [Authorize]
        [HttpGet("TransferMenu")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<ActionResult<TransferInfoDTO>> GetSectionTransferInfo(int bookId)
        {
            var chapterId = await _redis.StringGetAsync($"managerInfo:{UserId}");
            if (chapterId.IsNullOrEmpty)
            {
                return BadRequest("Session has time out");
            }
            
            var info = await _section.GetTransferInfoAsync(bookId);
            return Ok(info);
        }
        
        [Authorize]
        [HttpPost("AddChoice")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<ActionResult> AddChoiceToManager(int number, int flow, int choiceNumber, 
            int nextNumber, int nextFlow, string text, int nextChapterId = 0)
        {
            try
            {
                await _choiceService.CreateChoiceAsync(UserId, number, flow, choiceNumber, nextChapterId, nextNumber,
                    nextFlow, text);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
            return Ok();
        }
        
        [Authorize]
        [HttpPut("UpdateChoice")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<ActionResult> UpdateChoiceToManager(int number, int flow, int choiceNumber, 
            int nextNumber, int nextFlow, string text, int nextChapterId = 0)
        {
            try
            {
                await _choiceService.UpdateChoiceAsync(UserId, number, flow, choiceNumber, nextChapterId, nextNumber,
                    nextFlow, text);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
            return Ok();
        }
        
        [Authorize]
        [HttpDelete("DeleteChoice")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<ActionResult> DeleteChoiceToManager(int number, int flow, int choiceNumber)
        {
            try
            {
                await _choiceService.DeleteChoiceAsync(UserId, number, flow, choiceNumber);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
            return Ok();
        }
}