using AuthorVerseServer.Data.ControllerSettings;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NoteController : AuthorVerseController
    {
        private readonly INote _note;

        public NoteController(INote note)
        {
            _note = note;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<Note>>> GetNote()
        {
            var notes = await _note.GetNoteAsync();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(notes);
        }
    }
}
