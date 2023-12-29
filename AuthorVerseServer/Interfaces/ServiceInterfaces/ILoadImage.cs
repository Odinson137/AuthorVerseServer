namespace AuthorVerseServer.Interfaces.ServiceInterfaces
{
    public interface ILoadFile
    {
        string GetUniqueName(IFormFile file);
        Task CreateFileAsync(IFormFile file, string path, string imagesFolder);
    }
}
