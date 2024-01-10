using AuthorVerseServer.Data.ControllerSettings;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthorVerseServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArtController :  AuthorVerseController
{
    private readonly IArt _art;
    private readonly LoadFileService _fileService;
    public ArtController(IArt art, LoadFileService fileService)
    {
        _art = art;
        _fileService = fileService;
    }
    
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<ActionResult<ICollection<ArtDTO>>> GetArts(int bookId, int start, int end)
    {
        var arts = await _art.GetArtsAsync(bookId, start, end);
        return Ok(arts);
    }
    
    [Authorize]
    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<int>> AddArt(CreateArtDTO createArt)
    {
        var nameFile = _fileService.GetUniqueName(createArt.Image);
        await _fileService.CreateFileAsync(createArt.Image, nameFile, "arts");
        var art = new Art()
        {
            BookId = createArt.BookId,
            UserId = UserId,
            Url = nameFile,
        };
        
        await _art.AddArtAsync(art);

        await _art.SaveAsync();
        
        return Ok(art.ArtId);
    }

    [Authorize]
    [HttpDelete]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> DeleteArt(int artId)
    {
        var art = await _art.GetArtAsync(artId);
        if (art == null)
        {
            return BadRequest("Art not found");
        }
        if (art.User.Id == UserId)
        {
            return new UnauthorizedObjectResult("Not authorized");
        }
        
        await _art.DeleteArtAsync(artId);
        _fileService.DeleteFile(art.Url, "arts");
        return Ok();
    }
}