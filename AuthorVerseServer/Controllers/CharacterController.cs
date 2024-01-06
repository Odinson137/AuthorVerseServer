using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Interfaces.ServiceInterfaces;
using AuthorVerseServer.Models;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using AuthorVerseServer.Data.ControllerSettings;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : AuthorVerseController
    {
        private readonly ICharacter _character;
        private readonly CreateJWTtokenService _token;
        private readonly LoadFileService _loadImage;

        public CharacterController(ICharacter character, CreateJWTtokenService token, LoadFileService loadImage)
        {
            _character = character;
            _token = token;
            _loadImage = loadImage;
        }

        [Authorize]
        [HttpGet("{bookId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ICollection<BookCharacterDTO>>> GetBookCharacter(int bookId)
        {
            if (await _character.IsAuthorByBookIdAsync(bookId, UserId) == false)
                return BadRequest("You are not the Author!");

            var characters = await _character.GetCharactersAsync(bookId);

            return Ok(characters);
        }

        [Authorize]
        [HttpGet("{bookId}/{name}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ICollection<BookCharacterDTO>>> GetBookCharacterByName(int bookId, string name)
        {
            if (await _character.IsAuthorByBookIdAsync(bookId, UserId) == false)
                return BadRequest("You are not the Author!");

            var characters = await _character.GetCharactersByNameAsync(bookId, name);

            return Ok(characters);
        }

        [Authorize]
        [HttpPost("Book/{bookId}/{name}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<int>> AddCharacterToBook(int bookId, string name)
        {
            if (await _character.IsAuthorByBookIdAsync(bookId, UserId) == false)
                return BadRequest("You are not the Author!");

            if (await _character.ExistNameAsync(bookId, name) == true)
                return BadRequest("This name is already taken");

            var character = new Character
            {
                Name = name,
                BookId = bookId,
            };

            await _character.AddCharacterToBookAsync(character);

            await _character.SaveAsync();
            
            return Ok(character.CharacterId);
        }


        [Authorize]
        [HttpPost("Chapter/{chapterId}/{characterId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> AddCharacterToChapter(int chapterId, int characterId)
        {
            if (await _character.IsAuthorByCharacterIdAsync(characterId, UserId) == false)
                return BadRequest("You are not the Author!");

            if (await _character.ExistCharacterChaption(chapterId, characterId) == true)
                return BadRequest("This connection already exist");

            var characterChapter = new CharacterChapter
            {
                ChapterId = chapterId,
                CharacterId = characterId,
            };

            await _character.AddCharacterToChapterAsync(characterChapter);

            await _character.SaveAsync();

            return Ok();
        }

        [Authorize]
        [HttpPut("Text")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> UpdateCharacter(UpdateCharacterDTO characterDTO)
        {
            if (await _character.IsAuthorByCharacterIdAsync(characterDTO.CharacterId, UserId) == false)
                return BadRequest("You are not the Author!");

            var character = await _character.GetCharacterAsync(characterDTO.CharacterId);

            if (character == null)
                return NotFound("Character not found");

            character.Name = characterDTO.Name;
            character.Description = characterDTO.Description;

            await _character.SaveAsync();

            return Ok();
        }

        [Authorize]
        [HttpPut("Image")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> UpdateCharacterImage(UpdateCharacterImageDTO characterDTO)
        {
            if (await _character.IsAuthorByCharacterIdAsync(characterDTO.CharacterId, UserId) == false)
                return BadRequest("You are not the Author!");

            var character = await _character.GetCharacterAsync(characterDTO.CharacterId);

            if (character == null)
                return NotFound("Character not found");

            string nameFile = _loadImage.GetUniqueName(characterDTO.Image);
            await _loadImage.CreateFileAsync(characterDTO.Image, nameFile, "Images");
            character.CharacterImageUrl = nameFile;

            await _character.SaveAsync();

            return Ok();
        }



        [Authorize]
        [HttpDelete("{chapterId}/{characterId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> DeleteCharacterFromChapter(int chapterId, int characterId)
        {
            if (await _character.IsAuthorByCharacterIdAsync(characterId, UserId) == false)
                return BadRequest("You are not the Author!");

            await _character.DeleteCharacterFromChapterAsync(chapterId, characterId);

            return Ok();
        }


        [Authorize]
        [HttpDelete("{characterId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> DeleteCharacterFromBook(int characterId)
        {
            if (await _character.IsAuthorByCharacterIdAsync(characterId, UserId) == false)
                return BadRequest("You are not the Author!");

            await _character.DeleteCharacterFromChaptersAsync(characterId);
            await _character.DeleteCharacterFromBookAsync(characterId);

            return Ok();
        }
    }
}
