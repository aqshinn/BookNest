using BookNest.Business.Extensions;
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
        private const string _folderPath = "uploads/books";

        private readonly AppDbContext _context;
        private readonly IGenreService _genreService;
        private readonly IAuthorService _authorService;
        private readonly IWebHostEnvironment _env;
        private readonly IFileService _fileService;
        public BookController(AppDbContext context,
                       IGenreService genreService,
                       IAuthorService authorService,
                       IWebHostEnvironment env,
                       IFileService fileService)
        {
            _context = context;
            _genreService = genreService;
            _authorService = authorService;
            _env = env;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _context.Books
                .Where(b => !b.IsDeleted)
                .Include(b => b.BookGenres).ThenInclude(bg => bg.Genre)
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
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

            string imageFileName = null;

            if (model.Photo != null)
            {
                if (!model.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "Please select an image file.");
                    return View(model);
                }

                if (!model.Photo.CheckFileSize(2000))
                {
                    ModelState.AddModelError("Photo", "The image size cannot exceed 2MB.");
                    return View(model);
                }

                
                imageFileName = await _fileService.UploadAsync(model.Photo, _env.WebRootPath, _folderPath);
            }

            Book book = new Book
            {
                Title = model.Title,
                Description = model.Description,
                PageCount = model.PageCount,
                Language = model.Language,
                PublishedDate = model.PublishedDate,
                ImageUrl = imageFileName
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

            if (book == null) return NotFound();

            book.IsDeleted = true;


            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var book = await _context.Books
                .Include(b => b.BookGenres)
                .Include(b => b.BookAuthors)
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

            if (book == null) return NotFound();

            BookEditVM model = new BookEditVM
            {
                Title = book.Title,
                Description = book.Description,
                PageCount = book.PageCount,
                Language = book.Language,
                PublishedDate = book.PublishedDate,
                ExistingImageUrl = book.ImageUrl,

              
                GenreIds = book.BookGenres.Select(bg => bg.GenreId).ToList(),
                AuthorIds = book.BookAuthors.Select(ba => ba.AuthorId).ToList(),

               
                Genres = await _genreService.GetGenresForSelectAsync(),
                Authors = await _authorService.GetAuthorsForSelectAsync()
            };

            return View(model);
        }

        
        [HttpPost]
        public async Task<IActionResult> Update(int id, BookEditVM model)
        {
            
            if (!ModelState.IsValid)
            {
                model.Genres = await _genreService.GetGenresForSelectAsync();
                model.Authors = await _authorService.GetAuthorsForSelectAsync();
                return View(model);
            }

            var book = await _context.Books
                .Include(b => b.BookGenres)
                .Include(b => b.BookAuthors)
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

            if (book == null) return NotFound();

            
            if (model.NewPhoto != null)
            {
                
                if (!model.NewPhoto.CheckFileType("image/"))
                {
                    ModelState.AddModelError("NewPhoto", "File must be an image.");
                    model.Genres = await _genreService.GetGenresForSelectAsync();
                    model.Authors = await _authorService.GetAuthorsForSelectAsync();
                    return View(model);
                }
                if (!model.NewPhoto.CheckFileSize(2000))
                {
                    ModelState.AddModelError("NewPhoto", "Image size must be less than 2MB.");
                    model.Genres = await _genreService.GetGenresForSelectAsync();
                    model.Authors = await _authorService.GetAuthorsForSelectAsync();
                    return View(model);
                }

                
                if (!string.IsNullOrEmpty(book.ImageUrl))
                {
                    _fileService.Delete(_env.WebRootPath, _folderPath, book.ImageUrl);
                }

                
                book.ImageUrl = await _fileService.UploadAsync(model.NewPhoto, _env.WebRootPath, _folderPath);
            }

            book.Title = model.Title;
            book.Description = model.Description;
            book.PageCount = model.PageCount;
            book.Language = model.Language;
            book.PublishedDate = model.PublishedDate;

            
            book.BookGenres.Clear();
            if (model.GenreIds != null)
            {
                foreach (var genreId in model.GenreIds)
                {
                    book.BookGenres.Add(new BookGenre { GenreId = genreId });
                }
            }

            book.BookAuthors.Clear();
            if (model.AuthorIds != null)
            {
                foreach (var authorId in model.AuthorIds)
                {
                    book.BookAuthors.Add(new BookAuthor { AuthorId = authorId });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Details(int id)
        {
            var book = await _context.Books
                .Include(b => b.BookGenres).ThenInclude(bg => bg.Genre)
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

            if (book == null) return NotFound();

            return View(book);
        }
    }
}