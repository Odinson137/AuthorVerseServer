namespace AuthorVerseServer.Interfaces.ServiceInterfaces
{
    public interface ILoadImage
    {
        string GetUniqueName(IFormFile file);
        Task CreateImageAsync(IFormFile file, string path, string imagesFolder);
    }
}
