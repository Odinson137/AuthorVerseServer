using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Text.Json.Serialization;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuoteController : ControllerBase
    {
        private readonly IQuote _quote;
        private readonly IDatabase _redis;
        private readonly CreateJWTtokenService _jWTtokenService;
        public QuoteController(IQuote quote, IConnectionMultiplexer redisConnection, CreateJWTtokenService jWTtokenService)
        {
            _quote = quote;
            _redis = redisConnection.GetDatabase();
            _jWTtokenService = jWTtokenService;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ICollection<QuoteDTO>>> GetBookQuotes(int bookId, int page = 1)
        {
            if (--page < 0)
                return BadRequest("Zero count");

            string key = $"quotes{bookId}-{page}";
            string? quotesJson = await _redis.StringGetAsync(key);

            if (string.IsNullOrEmpty(quotesJson))
            {
                var quotes = await _quote.GetBookQuotesAsync(bookId, page);
                await _redis.StringSetAsync(key, JsonConvert.SerializeObject(quotes), TimeSpan.FromMinutes(15));
                return Ok(quotes);
            }

            var cacheQuotes = JsonConvert.DeserializeObject<ICollection<QuoteDTO>>(quotesJson);
            return Ok(cacheQuotes);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<int>> PostNewBookQuote(int bookId, string text)
        {
            string? userId = _jWTtokenService.GetIdFromToken(this.User);
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Token user is not correct");

            var quote = new BookQuote()
            {
                Text = text,
                BookId = bookId,
                UserId = userId,
            };

            await _quote.AddBookQuoteAsync(quote);
            await _quote.SaveAsync();

            return Ok(quote.BaseId);
        }

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> DeleteBookQuote(int quoteId)
        {
            await _quote.DeleteBookQuoteAsync(quoteId);
            return Ok();
        }
    }
}
