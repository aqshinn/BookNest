using BookNest.Data.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookNest.MVC.ViewComponents
{
    public class NewArrivalsViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public NewArrivalsViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Get newest books with all related data
            var books = await _context.Books
                .Where(b => !b.IsDeleted)
                .Include(b => b.BookGenres).ThenInclude(bg => bg.Genre)
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .Include(b => b.Reviews) // Include reviews to calculate ratings
                .OrderByDescending(b => b.Id) // Get newest books first (use Id if no CreatedAt)
                .Take(4)
                .ToListAsync();

            return View(books);
        }
    }
}
