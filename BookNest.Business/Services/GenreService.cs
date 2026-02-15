using BookNest.Business.Interfaces;
using BookNest.Core.Entities;
using BookNest.Data.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookNest.Business.Services
{
    public class GenreService : IGenreService
    {
        private readonly AppDbContext _context;

        public GenreService(AppDbContext context)
        {
           _context = context;
        }


        public async Task<List<Genre>> GetAllAsync()
        {
            return await _context.Genres
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }

        public async Task<Genre> GetByIdAsync(int id)
        {
            return await _context.Genres.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task CreateAsync(Genre genre)
        {
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Genre genre)
        {
            _context.Genres.Update(genre);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre != null)
            {
                genre.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<SelectListItem>> GetGenresForSelectAsync()
        {
            return await _context.Genres
                .Where(g => !g.IsDeleted)
                .Select(g => new SelectListItem
                {
                    Text = g.Name,
                    Value = g.Id.ToString()
                })
                .ToListAsync();
        }


    }
}
