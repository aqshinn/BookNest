using BookNest.Core.Entities;
using BookNest.Data.Data;
using BookNest.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookNest.MVC.Controllers
{
    public class ShelfController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ShelfController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // statusId = 1 (To Read), 2 (Currently Reading), 3 (Read)
        [Authorize]
        public async Task<IActionResult> Index(int statusId = 1)
        {
            var user = await _userManager.GetUserAsync(User);

            // Get only the user's books in the selected status
            var myShelf = await _context.ReadingLists
                .Include(rl => rl.Book)
                .ThenInclude(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .Where(rl => rl.AppUserId == user.Id && rl.StatusId == statusId)
                .OrderByDescending(rl => rl.AddedDate)
                .ToListAsync();

            var shelfVM = new ShelfVM
            {
                ReadingLists = myShelf,
                CurrentStatus = statusId
            };

            return View(shelfVM);
        }

        // ADD BOOK TO LIST OR CHANGE ITS STATUS
        [HttpPost]
        public async Task<IActionResult> UpdateShelf(int bookId, int newStatusId)
        {
            var user = await _userManager.GetUserAsync(User);

            // If not authenticated, redirect to login with return URL
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new 
                { 
                    returnUrl = Url.Action("Index", "Shelf", new { statusId = newStatusId })
                });
            }

            // Check if book exists
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                return NotFound("Book not found");
            }

            // Check if this book is already in the user's shelf
            var existingItem = await _context.ReadingLists
                .FirstOrDefaultAsync(rl => rl.AppUserId == user.Id && rl.BookId == bookId);

            if (existingItem != null)
            {
                // If it exists, just change its status
                existingItem.StatusId = newStatusId;
                if (newStatusId == 3) existingItem.FinishedDate = DateTime.Now; // If 'Read' status is selected, record the date
            }
            else
            {
                // If it doesn't exist, create a new one
                var newItem = new ReadingList
                {
                    AppUserId = user.Id,
                    BookId = bookId,
                    StatusId = newStatusId
                };
                _context.ReadingLists.Add(newItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { statusId = newStatusId });
        }

        // Delete book from shelf
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveFromShelf(int readingListId, int statusId)
        {
            var user = await _userManager.GetUserAsync(User);

            // Get the reading list item and verify it belongs to the current user
            var readingListItem = await _context.ReadingLists
                .FirstOrDefaultAsync(rl => rl.Id == readingListId && rl.AppUserId == user.Id);

            if (readingListItem == null)
            {
                return BadRequest("Book not found in your shelf");
            }

            // Remove the book from shelf
            _context.ReadingLists.Remove(readingListItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { statusId = statusId });
        }
    }
}