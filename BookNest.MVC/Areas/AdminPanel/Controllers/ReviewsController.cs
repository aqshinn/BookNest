using BookNest.Core.Entities;
using BookNest.Data.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookNest.MVC.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = "Admin")] // Only admins can access this controller
    public class ReviewsController : Controller
    {
        private readonly AppDbContext _context;

        public ReviewsController(AppDbContext context)
        {
            _context = context;
        }

        // 1. List all reviews
        public async Task<IActionResult> Index()
        {
            var reviews = await _context.Reviews
                .Include(r => r.Book)
                .Include(r => r.AppUser)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(reviews);
        }

        // 2. Delete a review and automatically recalculate rating
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null) return NotFound();

            var book = review.Book;

            // RECALCULATE RATING (DENORMALIZATION)
            if (book.ReviewCount <= 1)
            {
                // If deleting the last review, reset book rating to 0
                book.AverageRating = 0;
                book.ReviewCount = 0;
            }
            else
            {
                // Otherwise, subtract the deleted review's rating from the total
                double currentTotalScore = book.AverageRating * book.ReviewCount;
                double newTotalScore = currentTotalScore - review.Rating;

                book.ReviewCount -= 1;
                book.AverageRating = newTotalScore / book.ReviewCount;
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Review deleted successfully and book rating has been updated!";
            return RedirectToAction(nameof(Index));
        }
    }
}