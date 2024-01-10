using System.Collections;
using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AuthorVerseServer.Repository;

public class ArtRepository : IArt
{
    private readonly DataContext _context;
    public ArtRepository(DataContext context)
    {
        _context = context;
    }

    public Task<int> SaveAsync()
    {
        return _context.SaveChangesAsync();
    }

    public Task<ArtDTO?> GetArtAsync(int artId)
    {
        return _context.Arts
            .Where(a => a.ArtId == artId)
            .Select(a => new ArtDTO()
            {
                ArtId = a.ArtId,
                Url = a.Url,
                User = new UserDTO()
                {
                    Id = a.UserId,
                    UserName = a.User.UserName,
                },
            })
            .FirstOrDefaultAsync();
    }

    public Task<List<ArtDTO>> GetArtsAsync(int bookId, int start, int end)
    {
        var arts = _context.Arts
            .Where(a => a.BookId == bookId)
            .Skip(start - 1)
            .Take(end)
            .Select(a => new ArtDTO()
            {
                ArtId = a.ArtId,
                Url = a.Url,
                User = new UserDTO()
                {
                    Id = a.UserId,
                    UserName = a.User.UserName,
                },
            });
        
        return arts.ToListAsync();
    }

    public ValueTask<EntityEntry<Art>> AddArtAsync(Art art)
    {
        return _context.Arts.AddAsync(art);
    }

    public Task DeleteArtAsync(int artId)
    {
        return _context.Arts
            .Where(a => a.ArtId == artId)
            .ExecuteDeleteAsync();

    }
}