using BookingApp.Models;
using BookingApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService _userService;

        public AccountController(UserService userService)
        {
            _userService = userService;
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (_userService.EmailExists(model.Email))
            {
                ModelState.AddModelError("Email", "Ovaj email je već registrovan.");
                return View(model);
            }

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = model.Role
            };

            _userService.Create(user);
            await SignInUser(user);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _userService.GetByEmail(model.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Pogrešan email ili lozinka.");
                return View(model);
            }

            await SignInUser(user);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUser(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id!),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
        }
    }
}