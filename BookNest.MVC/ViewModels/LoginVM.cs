using System.ComponentModel.DataAnnotations;

namespace BookNest.MVC.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Username or Email is required")]
        public string UserNameOrEmail { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        public bool RememberMe { get; set; }
    }
}