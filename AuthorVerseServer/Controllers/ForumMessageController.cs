using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ForumMessageController : ControllerBase
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

            var message = new ForumMessage
            {
                BookId = sendMessage.BookId,
                Text = sendMessage.Text,
                UserId = sendMessage.UserId,
                ParrentMessageId = null,           
            };
            
            await _forum.AddForumMessageAsync(message);

            await _forum.SaveAsync();

            return Ok(message.MessageId);
        }

        //[HttpPost]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(400)]
        //public async Task<ActionResult<int>> PutMessage(string key)
        //{
        //    string? messageJson = await _redis.StringGetAsync($"put_message:{key}");
        //    if (string.IsNullOrEmpty(messageJson)) return NotFound("Value is not found");

        //    var sendMessage = JsonConvert.DeserializeObject<SendForumMessageDTO>(messageJson);
        //    if (sendMessage == null) return BadRequest("Bad data");

        //    var message = new ForumMessage
        //    {
        //        BookId = sendMessage.BookId,
        //        Text = sendMessage.Text,
        //        UserId = sendMessage.UserId,
        //        ParrentMessageId = sendMessage.AnswerId,
        //    };

        //    await _forum.AddForumMessageAsync(message);

        //    await _forum.SaveAsync();

        //    return Ok(message.MessageId);
        //}

        //[HttpPost]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(400)]
        //public async Task<ActionResult<int>> DeleteMessage(string key)
        //{
        //    string? messageJson = await _redis.StringGetAsync($"del_message:{key}");
        //    if (string.IsNullOrEmpty(messageJson)) return NotFound("Value is not found");
        //    string test = "asdasd";
        //    var sendMessage = JsonConvert.DeserializeObject<SendForumMessageDTO>(test);
        //    if (sendMessage == null) return BadRequest("Bad data");

        //    var message = new ForumMessage
        //    {
        //        BookId = sendMessage.BookId,
        //        Text = sendMessage.Text,
        //        UserId = sendMessage.UserId,
        //        ParrentMessageId = sendMessage.AnswerId,
        //    };

        //    await _forum.AddForumMessageAsync(message);

        //    await _forum.SaveAsync();

        //    return Ok(message.MessageId);
        //}
    }
}
