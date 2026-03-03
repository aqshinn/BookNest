
using BookNest.Core.Entities;
using BookNest.Data.Data;
using BookNest.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookNest.MVC.Controllers
{

    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            // 1. We find the trending book from the database (The one with the most reviews and the highest score)
            var trendingBook = await _context.Books
                .Where(b => !b.IsDeleted)
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .OrderByDescending(b => b.ReviewCount)
                .ThenByDescending(b => b.AverageRating)
                .FirstOrDefaultAsync();

            // 2. We put the data in our pocket (ViewBag) to send it to View
            ViewBag.TrendingBook = trendingBook;

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


        [Route("Home/Error")]
        public IActionResult Error(int? statusCode = null)
        {
            // Initialize our strictly-typed ViewModel
            var model = new ErrorViewModel
            {
                StatusCode = statusCode
            };

            // Assign messages based on the status code
            if (statusCode == 404)
            {
                model.ErrorMessage = "Looks like you've wandered off the path! The page or book you're looking for cannot be found on these shelves.";
            }
            else if (statusCode == 500)
            {
                model.ErrorMessage = "An unexpected system error occurred.";
            }
            else
            {
                model.ErrorMessage = "Something went wrong. Let's get you back to safety.";
            }

            // Pass the model to the view
            return View(model);
        }
    }
}
