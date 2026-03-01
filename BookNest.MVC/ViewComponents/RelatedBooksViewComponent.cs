using BookNest.Data.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookNest.MVC.ViewComponents
{
    public class RelatedBooksViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public RelatedBooksViewComponent(AppDbContext context)
        {
            _context = context;
        }

        // This method is automatically invoked when the ViewComponent is called
        public async Task<IViewComponentResult> InvokeAsync(int genreId, int currentBookId)
        {
            // Get 4 books from the same category, excluding the current book
            var relatedBooks = await _context.Books
                .Include(b => b.BookGenres).ThenInclude(bg => bg.Genre)
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .Include(b => b.Reviews)
                .Where(b => b.BookGenres.Any(bg => bg.GenreId == genreId) && 
                       b.Id != currentBookId && 
                       !b.IsDeleted)
                .OrderByDescending(b => b.Id) // Get the newest books first
                .Take(4) // Get only 4 books
                .ToListAsync();

            return View(relatedBooks);
        }
    }
}