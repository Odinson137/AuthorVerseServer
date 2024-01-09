using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AuthorVerseServer.Interfaces;

public interface IAdvertisement
{
    Task<int> SaveAsync();
    Task DeleteAdvertisementAsync(int id);
    Task<EarnDTO> GetAdvertisementAsync();
    Task<List<Advertisement>> GetAllAdvertisementsAsync();
    Task<int> UpdateEarningsAsync(int bookId, double addPrise);
    ValueTask<EntityEntry<Advertisement>> AddAdvertisementAsync(Advertisement advertisement);
}