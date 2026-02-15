using BookNest.Core.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookNest.Business.Interfaces
{
    public interface IGenreService
    {
        Task<List<Genre>> GetAllAsync();
        Task<Genre> GetByIdAsync(int id);
        Task CreateAsync(Genre genre);
        Task UpdateAsync(Genre genre);
        Task DeleteAsync(int id);
        Task<List<SelectListItem>> GetGenresForSelectAsync();
    }
}