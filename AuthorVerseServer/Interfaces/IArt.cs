using AuthorVerseServer.DTO;
using AuthorVerseServer.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AuthorVerseServer.Interfaces;

public interface IArt
{
    Task<int> SaveAsync();
    Task<ArtDTO?> GetArtAsync(int artId);
    Task<List<ArtDTO>> GetArtsAsync(int bookId, int start, int end);
    ValueTask<EntityEntry<Art>> AddArtAsync(Art art);
    Task DeleteArtAsync(int artId);
}