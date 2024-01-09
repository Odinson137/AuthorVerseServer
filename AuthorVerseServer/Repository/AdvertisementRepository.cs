using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AuthorVerseServer.Repository;

public class AdvertisementRepository : IAdvertisement
{
    private readonly DataContext _context;
    public AdvertisementRepository(DataContext context)
    {
        _context = context;
    }

    public Task<int> SaveAsync()
    {
        return _context.SaveChangesAsync();
    }

    public Task DeleteAdvertisementAsync(int id)
    {
        return _context.Advertisements
            .Where(ad => ad.AdvertisementId == id)
            .ExecuteDeleteAsync();
    }

    public Task<EarnDTO> GetAdvertisementAsync()
    {
        var randomValue = _context.Advertisements
            .Where(c => c.EndDate > DateTime.Now)
            .Where(c => c.StartDate < DateTime.Now)
            .OrderBy(ad => Guid.NewGuid())
            .Select(ad => new EarnDTO()
            {
                Earn = ad.Cost,
                AdvertisementDTO = new AdvertisementDTO()
                {
                    Link = ad.Link,
                    Url = ad.Url,
                }
            });

        return randomValue.FirstAsync();
    }

    public Task<List<Advertisement>> GetAllAdvertisementsAsync()
    {
        return _context.Advertisements.AsNoTracking().ToListAsync();
    }

    public Task<int> UpdateEarningsAsync(int bookId, double addPrise)
    {
        return _context.Books
            .Where(b => b.BookId == bookId)
            .ExecuteUpdateAsync(setter =>
                setter.SetProperty(b => b.Earnings, b=> b.Earnings + addPrise));
    }

    public ValueTask<EntityEntry<Advertisement>> AddAdvertisementAsync(Advertisement advertisement)
    {
        return _context.Advertisements.AddAsync(advertisement);
    }
}