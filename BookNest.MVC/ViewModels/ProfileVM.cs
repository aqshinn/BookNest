namespace BookNest.MVC.ViewModels
{
    public class ProfileVM
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? Bio { get; set; }
        public bool IsProfilePublic { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class EditProfileVM
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Bio { get; set; }
        public bool IsProfilePublic { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public string? CurrentProfileImageUrl { get; set; }
    }

    public class ChangePasswordVM
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
