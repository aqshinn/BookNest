
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookNest.Core.Entities;
using BookNest.Data.Data;

namespace BookNest.MVC.Controllers
{

    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Details(int id)
        {
            var book = await _context.Books
                .Where(b => !b.IsDeleted)
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .Include(b => b.BookGenres).ThenInclude(bg => bg.Genre)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) { return NotFound(); }

            return View(book);
        }

    }
}
