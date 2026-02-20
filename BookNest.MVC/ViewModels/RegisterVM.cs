using System.ComponentModel.DataAnnotations;

namespace BookNest.MVC.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Name and Surname is required")]
        [StringLength(50)]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Username is required")]
        [StringLength(20)]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match!")]
        public string ConfirmPassword { get; set; } = null!;
    }
}