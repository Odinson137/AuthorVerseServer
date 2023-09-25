using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserSelectedBookController : ControllerBase
    {
        private readonly IUserSelectedBook _userSelectedBook;

        public UserSelectedBookController(IUserSelectedBook usSelectedBook)
        {
            _userSelectedBook = usSelectedBook;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<UserSelectedBook>>> GetUserSelectedBookChapter()
        {
            var userSelectedBooks = await _userSelectedBook.GetUserSelectedBookAsync();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(userSelectedBooks);//Ок нужен чтобы работал код
        }
    }
}
