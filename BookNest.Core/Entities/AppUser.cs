
using Microsoft.AspNetCore.Identity;
namespace BookNest.Core.Entities
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
