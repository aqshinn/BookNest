using BookNest.Core.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace BookNest.Core.Entities
{
    public class Genre : BaseEntity
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Genre name cannot exceed 50 characters!")]
        public string Name { get; set; }
        public ICollection<BookGenre>? BookGenres { get; set; }
    }
}
