using AsyncAwaitBestPractices;
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
public class AdvertisementController : AuthorVerseController
{
    private readonly IAdvertisement _advertisement;
    private readonly LoadFileService _fileService;
    public AdvertisementController(IAdvertisement advertisement, LoadFileService fileService)
    {
        _advertisement = advertisement;
        _fileService = fileService;
    }
    
    [HttpPatch]
    [ProducesResponseType(200)]
    public async Task<ActionResult<AdvertisementDTO>> GetBookAdvertisement(int bookId)
    {
        EarnDTO advertisementEarn = await _advertisement.GetAdvertisementAsync();
        _advertisement.UpdateEarningsAsync(bookId, advertisementEarn.Earn).SafeFireAndForget();
        return Ok(advertisementEarn.AdvertisementDTO);
    }
    
    [Authorize(Roles = "Admin")]
    // [Authorize] // пока оставить так
    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<int>> AddAdvertisement(CreateAdvertisementDTO advertisementDTO)
    {
        var imageName = _fileService.GetUniqueName(advertisementDTO.Image);
        await _fileService.CreateFileAsync(advertisementDTO.Image, imageName, "advertisement");
        
        var advertisement = new Advertisement()
        {
            Url = imageName,
            Link = advertisementDTO.Link,
            Cost = advertisementDTO.Cost,
            StartDate = advertisementDTO.StartDate,
            EndDate = advertisementDTO.EndDate,
        };

        await _advertisement.AddAdvertisementAsync(advertisement);

        await _advertisement.SaveAsync();
        
        return Ok(advertisement.AdvertisementId);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("All")]
    [ProducesResponseType(200)]
    public async Task<ActionResult<ICollection<Advertisement>>> GetAllAdvertisement()
    {
        var advertisements = await _advertisement.GetAllAdvertisementsAsync();

        return Ok(advertisements);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete]
    [ProducesResponseType(200)]
    public async Task<ActionResult> DeleteAdvertisement(int id)
    {
        await _advertisement.DeleteAdvertisementAsync(id);
        return Ok();
    }
}