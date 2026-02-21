using BookNest.Core.Entities;
using BookNest.Data.Data;
using BookNest.MVC.Helpers;
using BookNest.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookNest.MVC.Controllers
{
    public class BookController : Controller
    {
        private readonly AppDbContext _context;

        public BookController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, int? genreId, int page = 1)
        {
            var query = _context.Books
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .Include(b => b.BookGenres).ThenInclude(bg => bg.Genre)
                .Where(b => !b.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(b => b.Title.ToLower().Contains(search.ToLower()) ||
                    b.BookAuthors.Any(ba => ba.Author.FullName.ToLower().Contains(search.ToLower()))
                );
            }

            if (genreId.HasValue)
            {
                query = query.Where(b => b.BookGenres.Any(bg => bg.GenreId == genreId));
            }

            int pageSize = 12; // 12 book each page

            var model = new BookIndexVM
            {
                Books = await PaginList<Book>.CreateAsync(query.OrderByDescending(b => b.Id), page, pageSize),
                Genres = await _context.Genres.Where(g => !g.IsDeleted).ToListAsync(),
                CurrentSearch = search,
                CurrentGenreId = genreId
            };

            return View(model);
        }
    }
}