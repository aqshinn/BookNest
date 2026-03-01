using BookNest.Data.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookNest.MVC.ViewComponents
{
    public class FeaturedBooksViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public FeaturedBooksViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Get featured books with all related data
            var books = await _context.Books
                .Where(b => !b.IsDeleted)
                .Include(b => b.BookGenres).ThenInclude(bg => bg.Genre)
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .Include(b => b.Reviews) // Include reviews to calculate ratings
                .OrderBy(r => Guid.NewGuid()) // Random order for featured
                .Take(4)
                .ToListAsync();

            return View(books);
        }
    }
}