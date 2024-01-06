using AsyncAwaitBestPractices;
using AuthorVerseServer.Interfaces.ServiceInterfaces;
using System.Collections;
using System.IO;

namespace AuthorVerseServer.Services
{
    public class LoadFileService
    {

        public string GetUniqueName(IFormFile file)
        {
            return $"Image_{DateTime.Now:yyyyMMdd_HHmmss}{Path.GetExtension(file.FileName)}";
        }

        public string GetUniqueName(string extension)
        {
            return $"Image_{DateTime.Now:yyyyMMdd_HHmmss}{extension}";
        }

        public async Task CreateFileAsync(IFormFile file, string nameFile, string imagesFolder)
        {
            await using (var stream = File.Create($@"./wwwroot/api/{imagesFolder}/{nameFile}"))
            {
                 await file.CopyToAsync(stream);
            }
        }

#if DEBUG
        public async Task CreateFileAsync(byte[] file, string nameFile, string imagesFolder)
        {
            await using (var stream = File.Create($@"../../../wwwroot/api/{imagesFolder}/{nameFile}"))
            {
                await stream.WriteAsync(file, 0, file.Length);
            }
        }

        public void DeleteFile(string fileName, string folderPath)
        {
            File.Delete($@"../../../wwwroot/api/{folderPath}/{fileName}");
        }

#else
        public async Task CreateFileAsync(byte[] file, string nameFile, string imagesFolder)
        {
            await using (var stream = File.Create($@"./wwwroot/api/{imagesFolder}/{nameFile}"))
            {
                stream.Write(file, 0, file.Length);
            }
        }
        
        public void DeleteFile(string fileName, string folderPath)
        {
            File.Delete($@"./wwwroot/api/{folderPath}/{fileName}");
        }
#endif

    }
}
