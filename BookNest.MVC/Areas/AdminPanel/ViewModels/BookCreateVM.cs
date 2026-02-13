using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BookNest.MVC.Areas.AdminPanel.ViewModels
{
    public class BookCreateVM
    {
        [Required(ErrorMessage = "The name of the book is required!")]
        public string Title { get; set; }

        public string Description { get; set; }

        public int? PageCount { get; set; }
        public string? Language { get; set; }
        public DateTime? PublishedDate { get; set; }


        [Required(ErrorMessage = "At least one genre must be selected!")]
        public List<int> GenreIds { get; set; }

        [Required(ErrorMessage = "At least one author must be selected!")]
        public List<int> AuthorIds { get; set; }

        public List<SelectListItem>? Genres { get; set; }
        public List<SelectListItem>? Authors { get; set; }
    }
}