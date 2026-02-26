using BookNest.Core.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace BookNest.Core.Entities
{
    public class Review : BaseEntity
    {
        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;

        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        [Range(1, 5)]
        public int Rating { get; set; } // 1 to 5
        public string Comment { get; set; } = null!;
    }
}
