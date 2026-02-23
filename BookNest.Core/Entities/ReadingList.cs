using System.ComponentModel.DataAnnotations;

namespace BookNest.Core.Entities
{
    public class ReadingList
    {
        public int Id { get; set; }

        [Required]
        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;

        [Required]
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        // Book's shelf status: 1 = To Read, 2 = Currently Reading, 3 = Read
        [Required]
        public int StatusId { get; set; }

        public DateTime AddedDate { get; set; } = DateTime.Now;
        public DateTime? FinishedDate { get; set; }
    }
}