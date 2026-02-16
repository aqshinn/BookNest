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
            var books = await _context.Books
                .Where(b => !b.IsDeleted)
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author) 
                .OrderByDescending(b => b.PublishedDate) 
                .Take(4) 
                .ToListAsync();

            return View(books);
        }
    }
}
