using BookNest.Business.Interfaces;
using BookNest.Core.Entities;
using BookNest.Data.Data;
using BookNest.MVC.Areas.AdminPanel.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookNest.MVC.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class BookController : Controller
    {
        private readonly IGenreService _genreService;
        private readonly IAuthorService _authorService;
        private readonly AppDbContext _context;

        public BookController(IGenreService genreService, IAuthorService authorService, AppDbContext context)
        {
            _genreService = genreService;
            _authorService = authorService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _context.Books
                .Include(b => b.BookGenres).ThenInclude(bg => bg.Genre) 
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .OrderByDescending(b => b.Id)
                .ToListAsync();

            return View(books);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var genres = await _genreService.GetAllAsync();
            var authors = await _authorService.GetAllAsync();

            BookCreateVM model = new BookCreateVM
            {
                Genres = genres.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList(),
                Authors = authors.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.FullName }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookCreateVM model)
        {
            
            if (!ModelState.IsValid)
            {
                
                var genres = await _genreService.GetAllAsync();
                var authors = await _authorService.GetAllAsync();

                model.Genres = genres.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
                model.Authors = authors.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.FullName }).ToList();

                return View(model);
            }

            Book book = new Book
            {
                Title = model.Title,
                Description = model.Description,
                PageCount = model.PageCount,
                Language = model.Language,
                PublishedDate = model.PublishedDate
            };


            book.BookGenres = new List<BookGenre>();
            foreach (var genreId in model.GenreIds)
            {
                book.BookGenres.Add(new BookGenre
                {
                    GenreId = genreId,
                    Book = book 
                });
            }

            book.BookAuthors = new List<BookAuthor>();
            foreach (var authorId in model.AuthorIds)
            {
                book.BookAuthors.Add(new BookAuthor
                {
                    AuthorId = authorId,
                    Book = book
                });
            }

            
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}