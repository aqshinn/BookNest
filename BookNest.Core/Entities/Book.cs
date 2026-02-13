using BookNest.Core.Entities.Common;


namespace BookNest.Core.Entities
{
    public class Book : BaseEntity
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public int PageCount { get; set; }
        public string ISBN { get; set; }
        public DateTime PublishedDate { get; set; }
        public string Language { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }

        public ICollection<BookImage> BookImages { get; set; }
        public ICollection<BookGenre> BookGenres { get; set; }

    }
}
