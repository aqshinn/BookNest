using BookNest.Core.Entities.Common;

namespace BookNest.Core.Entities
{
    public class Book : BaseEntity
    {
        public string Title { get; set; }
        public string? Description { get; set; }

        public int? PageCount { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string? Language { get; set; }


        public string? ImageUrl { get; set; } 

        public ICollection<BookGenre>? BookGenres { get; set; }
        public ICollection<BookAuthor>? BookAuthors { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}