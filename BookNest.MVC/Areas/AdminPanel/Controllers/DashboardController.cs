using BookNest.Data.Data;
using BookNest.MVC.Areas.AdminPanel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookNest.MVC.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        public DashboardController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var model = new DashboardVM
            {
                TotalBooks = await _context.Books.CountAsync(),
                TotalAuthors = await _context.Authors.CountAsync(),
                TotalGenres = await _context.Genres.CountAsync(),

                // 
                //TotalReviews = await _context.Reviews.CountAsync(),

                LatestBookTitle = await _context.Books
                    .OrderByDescending(b => b.Id)
                    .Select(b => b.Title)
                    .FirstOrDefaultAsync() ?? "No books yet",

                AdminName = User.Identity?.Name ?? "Admin"
            };

            return View(model);
        }
    }
}
