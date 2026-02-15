using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BookNest.MVC.Areas.AdminPanel.ViewModels
{
    public class BookEditVM
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int? PageCount { get; set; }
        public string? Language { get; set; }
        public DateTime? PublishedDate { get; set; }

        public string? ExistingImageUrl { get; set; }

        public IFormFile? NewPhoto { get; set; }


        public List<int> GenreIds { get; set; }
        public List<int> AuthorIds { get; set; }

        public List<SelectListItem>? Genres { get; set; }
        public List<SelectListItem>? Authors { get; set; }
    }
}