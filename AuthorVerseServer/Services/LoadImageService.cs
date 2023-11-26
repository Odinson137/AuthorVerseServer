using AuthorVerseServer.Interfaces.ServiceInterfaces;

namespace AuthorVerseServer.Services
{
    public class LoadImageService : ILoadImage
    {
        public string GetUniqueName(IFormFile file)
        {
            return $"Image_{DateTime.Now:yyyyMMdd_HHmmss}{Path.GetExtension(file.FileName)}";
        }

        public async Task CreateImageAsync(IFormFile file, string nameFile, string imagesFolder)
        {
            using (var stream = File.Create($"wwwroot\\Api\\{imagesFolder}\\{nameFile}"))
            {
                await file.CopyToAsync(stream);
            }
        }
    }
}
