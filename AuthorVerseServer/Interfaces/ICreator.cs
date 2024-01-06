using AuthorVerseServer.Data.JsonModels;
using AuthorVerseServer.Models;
using AuthorVerseServer.Models.ContentModels;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AuthorVerseServer.Interfaces;

public interface ICreator
{
    public Task<int> SaveAsync();
    
    public ValueTask<EntityEntry> AddContentAsync<T>(T content);
    public void DeleteContent(ContentBase content);
    public void DeleteSection(ChapterSection chapter);
    public Task<ChapterSection> GetSectionAsync(int chapterId, int number, int flow);
    // возвращает макимальный номер секции для данного потока
    public Task<int> CheckAddingNewSectionAsync(int chapterId, int flow);
    public Task<bool> CheckUpdatingNewSectionAsync(int chapterId, int number, int flow);
    public Task<ExistSection> CheckAddingNewChoiceAsync(int chapterId, int number, int flow);
    public Task<bool> CheckExistNextSectionAsync(int chapterId, int number, int flow);
}