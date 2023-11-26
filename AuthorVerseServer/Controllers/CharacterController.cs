using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacter _character;

        public CharacterController(ICharacter character)
        {
            _character = character;

        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ICollection<Character>>> GetCharacter()
        {
            var characters = await _character.GetCharacterAsync();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(characters);
        }
    }
}
