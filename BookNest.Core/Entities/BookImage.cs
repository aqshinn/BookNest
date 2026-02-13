using BookNest.Core.Entities.Common;


namespace BookNest.Core.Entities
{
    public class BookImage : BaseEntity
    {
        public string ImageUrl { get; set; }
        public bool IsMain { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
