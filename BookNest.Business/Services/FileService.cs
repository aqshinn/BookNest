using BookNest.Business.Interfaces;
using Microsoft.AspNetCore.Http;
namespace BookNest.Business.Services
{
    public class FileService : IFileService
    {
        public async Task<string> UploadAsync(IFormFile file, string root, string folder)
        {
            string folderPath = Path.Combine(root, folder);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileName = Guid.NewGuid().ToString() + "-" + file.FileName;

            string fullPath = Path.Combine(folderPath, fileName);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }

        public void Delete(string root, string folder, string fileName)
        {
            string path = Path.Combine(root, folder, fileName);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }


    }
}
