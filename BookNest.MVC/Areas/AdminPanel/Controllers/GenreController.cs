using BookNest.Business.Interfaces;
using BookNest.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookNest.MVC.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = "Admin")]
    public class GenreController : Controller
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        public async Task<IActionResult> Index()
        {
            var genres = await _genreService.GetAllAsync();
            return View(genres);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Genre genre)
        {
            if (!ModelState.IsValid)
            {
                return View(genre); 
            }

            await _genreService.CreateAsync(genre);
            TempData["Success"] = "Genre has been created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var genre = await _genreService.GetByIdAsync(id);
            if (genre == null) return NotFound();

            return View(genre);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Genre genre)
        {
            
            if (!ModelState.IsValid) return View(genre);

            var existingGenre = await _genreService.GetByIdAsync(id);
            if (existingGenre == null) return NotFound();

            existingGenre.Name = genre.Name;

            await _genreService.UpdateAsync(existingGenre);
            TempData["Success"] = "Genre has been updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Delete(int id)
        {
            await _genreService.DeleteAsync(id);
            TempData["Success"] = "Genre has been deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

    }
}
