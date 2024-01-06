using AuthorVerseServer.Data.ControllerSettings;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ForumMessageController : AuthorVerseController
    {
        private readonly IForumMessage _forum;
        private readonly IDatabase _redis;
        public ForumMessageController(IForumMessage forum, IConnectionMultiplexer redisConnection)
        {
            _forum = forum;
            _redis = redisConnection.GetDatabase();
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ICollection<ForumMessageDTO>>> GetMessages(int bookId, int page = 1)
        {
            if (--page < 0)
            {
                return BadRequest("Page in not correct");
            }

            var messages = await _forum.GetForumMessagesAsync(bookId, page);
            return Ok(messages);
        }

        [HttpGet("GetToParent")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ICollection<ForumMessageDTO>>> GetToParentMessages(int bookId, int lastMessageId, int parentMessageId)
        {
            var messages = await _forum.GetToParentMessagesAsync(bookId, lastMessageId, parentMessageId);
            return Ok(messages);
        }


        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<int>> AddMessage(string key)
        {
            string? messageJson = await _redis.StringGetAsync($"add_message:{key}");
            if (string.IsNullOrEmpty(messageJson)) return NotFound("Value is not found");

            var sendMessage = JsonConvert.DeserializeObject<SendForumMessageDTO>(messageJson);
            if (sendMessage == null) return BadRequest("Bad data");

            int messageId = await _forum.AddForumMessageProcedureAsync(sendMessage);

            return Ok(messageId);
        }

        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<int>> PutMessage(string key)
        {
            string? messageJson = await _redis.StringGetAsync($"put_message:{key}");
            if (string.IsNullOrEmpty(messageJson)) return NotFound("Value is not found");

            var changeTextDTO = JsonConvert.DeserializeObject<ChangeTextDTO>(messageJson);
            if (changeTextDTO == null) return BadRequest("Bad data");

            var message = await _forum.GetForumMessageAsync(changeTextDTO.MessageId);

            message.Text = changeTextDTO.NewText;

            await _forum.SaveAsync();
            return Ok();
        }

        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<int>> DeleteMessage(string key)
        {
            string? messageJson = await _redis.StringGetAsync($"delete_message:{key}");
            if (string.IsNullOrEmpty(messageJson)) return NotFound("Value is not found");

            var forumMessage = JsonConvert.DeserializeObject<DeleteMessageDTO>(messageJson);
            if (forumMessage == null) return BadRequest("Bad data");

            if (!await _forum.CheckUserMessageExistAsync(forumMessage.MessageId, forumMessage.UserId))
            {
                return NotFound("Comment not found, or you are not the author");
            }

            using (var transaction = _forum.StartTransaction())
            {
                await _forum.ChangeParentMessage(forumMessage.MessageId);
                await _forum.DeleteMessageAsync(forumMessage.MessageId);
            }

            return Ok();
        }
    }
}
