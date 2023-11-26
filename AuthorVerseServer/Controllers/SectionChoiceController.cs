using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SectionChoiceController : ControllerBase
    {
        private readonly ISectionChoice _sectionChoice;

        public SectionChoiceController(ISectionChoice sectionChoice)
        {
            _sectionChoice = sectionChoice;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<SectionChoice>>> GetSectionChoice()
        {
            var sectionChoices = await _sectionChoice.GetSectionChoiceAsync();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(sectionChoices);//Ок нужен чтобы работал код
        }
    }
}
