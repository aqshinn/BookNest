using BookNest.Core.Entities;
using BookNest.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookNest.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IWebHostEnvironment _env;
        private const string _profileFolder = "uploads/profiles";

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _env = env;
        }

        //Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid) return View(model);

            AppUser user = new AppUser
            {
                FullName = model.FullName,
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        //Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByNameAsync(model.UserNameOrEmail) ??
                       await _userManager.FindByEmailAsync(model.UserNameOrEmail);

            if (user == null)
            {
                ModelState.AddModelError("", "Username (or Email) or Password is incorrect!");
                return View(model);
            }

            // Check the login process
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username (or Email) or Password is incorrect!");
                return View(model);
            }

            // Check if user is admin
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            // If coming from another page, redirect there
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Redirect admin users to admin dashboard
            if (isAdmin)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "AdminPanel" });
            }

            // Regular users go to home
            return RedirectToAction("Index", "Home");
        }
        

        //Logout
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // Admin Login
        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AdminLogin(LoginVM model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByNameAsync(model.UserNameOrEmail) ??
                       await _userManager.FindByEmailAsync(model.UserNameOrEmail);

            if (user == null)
            {
                ModelState.AddModelError("", "Admin credentials incorrect!");
                return View(model);
            }

            // Check if user is admin
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (!isAdmin)
            {
                ModelState.AddModelError("", "You don't have admin privileges!");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Admin credentials incorrect!");
                return View(model);
            }

            return RedirectToAction("Index", "Dashboard", new { area = "AdminPanel" });
        }

        // VIEW PROFILE
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new ProfileVM
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName,
                ProfileImageUrl = user.ProfileImageUrl,
                Bio = user.Bio,
                IsProfilePublic = user.IsProfilePublic,
                CreatedDate = user.CreatedDate
            };
            return View(model);
        }

        // EDIT PROFILE
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new EditProfileVM
            {
                FullName = user.FullName,
                Email = user.Email,
                Bio = user.Bio,
                IsProfilePublic = user.IsProfilePublic,
                CurrentProfileImageUrl = user.ProfileImageUrl
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileVM model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (!ModelState.IsValid)
            {
                model.CurrentProfileImageUrl = user.ProfileImageUrl;
                return View(model);
            }

            //update basic info
            user.FullName = model.FullName;
            user.Email = model.Email;
            user.Bio = model.Bio;
            user.IsProfilePublic = model.IsProfilePublic;

            // Handle profile image upload
            if (model.ProfileImage != null)
            {
                //validate file
                if (!model.ProfileImage.ContentType.StartsWith("image/"))
                {
                    ModelState.AddModelError("ProfileImage", "File must be an image");
                    model.CurrentProfileImageUrl = user.ProfileImageUrl;
                    return View(model);
                }

                if (model.ProfileImage.Length > 5 * 1024 * 1024) // 5 MB limit
                {
                    ModelState.AddModelError("ProfileImage", "Image size cannot exceed 5MB");
                    model.CurrentProfileImageUrl = user.ProfileImageUrl;
                    return View(model);
                }

                //delete old image if exists
                if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                {
                    var oldImagePath = Path.Combine(_env.WebRootPath, "uploads/profiles", user.ProfileImageUrl);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Save new image
                var fileName = $"{user.Id}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(model.ProfileImage.FileName)}";
                var folderPath = Path.Combine(_env.WebRootPath, _profileFolder);

                // Create folder if doesn't exist
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var filePath = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(stream);
                }

                user.ProfileImageUrl = fileName;
            }

            // Update email if changed
            if (user.Email != model.Email)
            {
                var emailExists = await _userManager.FindByEmailAsync(model.Email);
                if (emailExists != null && emailExists.Id != user.Id)
                {
                    ModelState.AddModelError("Email", "Email already in use");
                    model.CurrentProfileImageUrl = user.ProfileImageUrl;
                    return View(model);
                }

                user.UserName = model.Email;
                user.NormalizedUserName = model.Email.ToUpper();
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                model.CurrentProfileImageUrl = user.ProfileImageUrl;
                return View(model);
            }

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Profile));
        }

        // CHANGE PASSWORD
        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match");
                return View(model);
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            TempData["Success"] = "Password changed successfully!";
            return RedirectToAction(nameof(Profile));
        }

        // DELETE ACCOUNT - POST
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Delete profile image
            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
            {
                var imagePath = Path.Combine(_env.WebRootPath, "uploads/profiles", user.ProfileImageUrl);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();
                TempData["Success"] = "Account deleted successfully";
                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = "Failed to delete account";
            return RedirectToAction(nameof(Profile));
        }

        // DELETE PROFILE PHOTO
        
    }
}