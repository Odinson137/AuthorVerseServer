namespace AuthorVerseServer.Interfaces.ServiceInterfaces.SectionCreateManager;

public interface ICudChoiceOperations
{
    ValueTask CreateChoiceAsync(string userId, int number, int flow, int choiceNumber, int nextChapterId,
        int nextNumber, int nextFlow,
        string text);
    ValueTask UpdateChoiceAsync(string userId, int number, int flow, int choiceNumber, int nextChapterId,
        int nextNumber, int nextFlow, string text);
    ValueTask DeleteChoiceAsync(string userId, int number, int flow, int choiceNumber);
}