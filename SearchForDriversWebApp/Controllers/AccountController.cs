using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SearchForDriversWebApp.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SearchForDriversWebApp.ViewModels;

namespace SearchForDriversWebApp.Controllers
{
    public class AccountController : Controller
    {
        private DriverDbContext db;
        public AccountController(DriverDbContext context)
        {
            db = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
                if (user != null)
                {
                    await Authenticate(model.Email); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некоректний логін або пароль");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    // добавляєм користувача в бд
                    db.Users.Add(new User { Username = model.Username, Phone = model.Phone, Email = model.Email, Password = model.Password, Role = "User" });
                    db.SaveChanges();

                    await Authenticate(model.Email);

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некоректний логін або пароль");
            }
            return View(model);
        }

        private async Task Authenticate(string userName)
        {
            User user = await db.Users.FirstOrDefaultAsync(u => u.Email == userName);

            if (user == null)
            {
                // Обробка помилки - користувача не знайдено
                return;
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
    };

            if (!string.IsNullOrEmpty(user.Role))
            {
                claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role));
            }

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}