using BookNest.Business.Interfaces;
using BookNest.Core.Entities;
using BookNest.Data.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookNest.Business.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly AppDbContext _context;

        public AuthorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Author>> GetAllAsync()
        {
            
            return await _context.Authors
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }

        public async Task<Author> GetByIdAsync(int id)
        {
            return await _context.Authors.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task CreateAsync(Author author)
        {
            await _context.Authors.AddAsync(author);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Author author)
        {
            _context.Authors.Update(author);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author != null)
            {
                author.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<SelectListItem>> GetAuthorsForSelectAsync()
        {
            return await _context.Authors
                .Where(a => !a.IsDeleted)
                .Select(a => new SelectListItem
                {
                    Text = a.FullName,
                    Value = a.Id.ToString()
                })
                .ToListAsync();
        }
    }
}