using BookNest.Business.Interfaces;
using BookNest.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookNest.MVC.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = "Admin")]
    public class AuthorController : Controller
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        public async Task<IActionResult> Index()
        {
            var authors = await _authorService.GetAllAsync();
            return View(authors);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Author author)
        {
            

            if (!ModelState.IsValid) return View(author);

            await _authorService.CreateAsync(author);
            TempData["Success"] = "Author has been created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var author = await _authorService.GetByIdAsync(id);
            if (author == null) return NotFound();

            return View(author);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Author author)
        {
           

            if (!ModelState.IsValid) return View(author);

            var existingAuthor = await _authorService.GetByIdAsync(id);
            if (existingAuthor == null) return NotFound();

            existingAuthor.FullName = author.FullName;
            existingAuthor.Biography = author.Biography;

            await _authorService.UpdateAsync(existingAuthor);
            TempData["Success"] = "Author has been updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _authorService.DeleteAsync(id);
            TempData["Success"] = "Author has been deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}