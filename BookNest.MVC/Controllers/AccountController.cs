using BookNest.Core.Entities;
using BookNest.MVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookNest.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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

    }
}