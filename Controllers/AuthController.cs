using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using SafeVault.Models;
using SafeVault.Helpers;

namespace SafeVault.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;

        public AuthController(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid || !ValidationHelper.IsValidInput(model.Username, "-_.") || ValidationHelper.ContainsXss(model.Username))
            {
                ModelState.AddModelError("", "Invalid input detected.");
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: model.RememberMe);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid || !ValidationHelper.IsValidInput(model.Username, "-_.") || ValidationHelper.ContainsXss(model.Username) || !ValidationHelper.IsValidXSSInput(model.Email))
            {
                ModelState.AddModelError("", "Invalid input detected.");
                return View(model);
            }

            var existingUser = await _userManager.FindByNameAsync(model.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "Username already exists.");
                return View(model);
            }

            var user = new UserModel
            {
                UserName = model.Username,
                Email = model.Email,
                PasswordHash = _userManager.PasswordHasher.HashPassword(null, model.Password) // Using Identity's password hashing
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Registration failed.");
                return View(model);
            }

            await _userManager.AddToRoleAsync(user, "User");
            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}