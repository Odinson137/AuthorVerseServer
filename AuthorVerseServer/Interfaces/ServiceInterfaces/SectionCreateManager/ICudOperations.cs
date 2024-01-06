namespace AuthorVerseServer.Interfaces.ServiceInterfaces.SectionCreateManager;

public interface ICudOperations
{
    ValueTask CreateSectionAsync(string userId, int number, int flow, object value);
    ValueTask UpdateSectionAsync(string userId, int number, int flow, object value);
    ValueTask DeleteSectionAsync(string userId, int number, int flow);
}