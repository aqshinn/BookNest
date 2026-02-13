using BookNest.Core.Entities.Common;

namespace BookNest.Core.Entities
{
    public class BookAuthor : BaseEntity
    {
        public int BookId { get; set; }
        public Book Book { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }
}