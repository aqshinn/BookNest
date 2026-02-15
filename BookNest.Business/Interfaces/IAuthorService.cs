using BookNest.Core.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookNest.Business.Interfaces
{
    public interface IAuthorService
    {
        Task<List<Author>> GetAllAsync();
        Task<Author> GetByIdAsync(int id);
        Task CreateAsync(Author author);
        Task UpdateAsync(Author author);
        Task DeleteAsync(int id);
        Task<List<SelectListItem>> GetAuthorsForSelectAsync();
    }
}
