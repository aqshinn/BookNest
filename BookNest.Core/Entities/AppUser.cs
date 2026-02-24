
using Microsoft.AspNetCore.Identity;
namespace BookNest.Core.Entities
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? Bio { get; set; }
        public bool IsProfilePublic { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
