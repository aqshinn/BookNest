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
            
            var books = await _context.Books
                .Where(b => !b.IsDeleted)
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .OrderBy(r => Guid.NewGuid())
                .Take(4)
                .ToListAsync();

            return View(books);
        }
    }
}