using Microsoft.AspNetCore.Http;

namespace BookNest.Business.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadAsync(IFormFile file, string root, string folder);
         void Delete(string root, string folder, string fileName);
    }
}
