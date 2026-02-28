using BookNest.Core.Entities;
using BookNest.Data.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace BookNest.MVC.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        

        public ReviewsController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("{bookId}")]
        public async Task<IActionResult> GetReviews(int bookId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.BookId == bookId)
                .Include(r => r.AppUser)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new {
                    id = r.Id,
                    userId = r.AppUserId,
                    userName = r.AppUser.FullName,
                    userImage = r.AppUser.ProfileImageUrl,
                    rating = r.Rating,
                    comment = r.Comment,
                    date = r.CreatedAt  
                })
                .ToListAsync();

            return Ok(reviews);

            
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview([FromBody] ReviewCreateDTO model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized(new { message = "User not found." });

            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == model.BookId && !b.IsDeleted);
            if (book == null) return NotFound(new { message = "Book not found." });

            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.BookId == model.BookId && r.AppUserId == userId);

            if (existingReview != null)
            {
                return BadRequest(new { message = "You have already reviewed this book. You can only edit your existing review." });
            }


            var newReview = new Review
            {
                BookId = model.BookId,
                AppUserId = userId,
                Rating = model.Rating,
                Comment = model.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Reviews.AddAsync(newReview);

            double currentTotalScore = book.AverageRating * book.ReviewCount;
            double newTotalScore = currentTotalScore + model.Rating;

            book.ReviewCount += 1;
            book.AverageRating = newTotalScore / book.ReviewCount;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Review added successfully!" });
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> EditReview([FromBody] ReviewCreateDTO model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized(new { message = "User not found." });

            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == model.BookId && !b.IsDeleted);
            if (book == null) return NotFound(new { message = "Book not found." });

            var existingReview = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.BookId == model.BookId && r.AppUserId == userId);
            if (existingReview == null)
            {
                return NotFound(new { message = "Your review for this book was not found." });
            }

            // DENORMALIZATION:
            if (existingReview.Rating != model.Rating)
            {
                // The formula: (Total Score - Old Rating + New Rating) / Review Count
                double currentTotalScore = book.AverageRating * book.ReviewCount;
                double newTotalScore = currentTotalScore - existingReview.Rating + model.Rating;

                book.AverageRating = newTotalScore / book.ReviewCount;
            }

            // Update the review
            existingReview.Rating = model.Rating;
            existingReview.Comment = model.Comment;
            existingReview.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Review updated successfully!" });
        }
    }







    public class ReviewCreateDTO
    {
        [Required]
        public int BookId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Comment cannot be empty.")]
        public string Comment { get; set; } = null!;
    }
}
