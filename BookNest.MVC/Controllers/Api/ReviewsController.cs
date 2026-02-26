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

            var bookExists = await _context.Books.AnyAsync(b => b.Id == model.BookId && !b.IsDeleted);
            if (!bookExists) return NotFound(new { message = "Book not found." });

            var newReview = new Review
            {
                BookId = model.BookId,
                AppUserId = userId,
                Rating = model.Rating,
                Comment = model.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Reviews.AddAsync(newReview);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Review added successfully!" });
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
