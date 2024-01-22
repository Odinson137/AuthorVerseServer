using AsyncAwaitBestPractices;
using AuthorVerseServer.Interfaces.ServiceInterfaces;
using System.Collections;
using System.IO;
using Path = System.IO.Path;

namespace AuthorVerseServer.Services
{
    public class LoadFileService
    {
        
        // #if DEBUG
        //         private string path = "../../../wwwroot/api";
        // # else 
                private string path = "./wwwroot/api";
        // #endif

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
            await using var stream = File.Create($@"{path}/{imagesFolder}/{nameFile}");
            await file.CopyToAsync(stream);
        }

        public async Task CreateFileAsync(byte[] file, string nameFile, string imagesFolder)
        {
            await using var stream = File.Create($@"{path}/{imagesFolder}/{nameFile}");
            await stream.WriteAsync(file, 0, file.Length);
        }
        
        public async Task CreateFileAsync(IFile file, string nameFile, string imagesFolder, string? newPath = null)
        {
            await using var fileStream =
                new FileStream($@"{newPath ?? path}/{imagesFolder}/{nameFile}", FileMode.Create);
            await file.CopyToAsync(fileStream);
        }

        public void DeleteFile(string fileName, string folderPath)
        {
            File.Delete($@"{path}/{folderPath}/{fileName}");
        }
    }
}
