using BookNest.Core.Entities.Common;

namespace BookNest.Core.Entities
{
    public class Author : BaseEntity
    {
        public string FullName { get; set; }
        public string? Biography { get; set; }
        public string? PhotoUrl { get; set; }
        public ICollection<Book>? Books { get; set; }
    }
}
