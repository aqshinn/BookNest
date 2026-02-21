using BookNest.Core.Entities;
using BookNest.MVC.Helpers;

namespace BookNest.MVC.ViewModels
{
    public class BookIndexVM
    {
        public PaginList<Book> Books { get; set; } = null!;
        public List<Genre> Genres { get; set; } = null!;
        public string? CurrentSearch { get; set; }
        public int? CurrentGenreId { get; set; }
    }
}