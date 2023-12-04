using System.Security.Claims;
using AirlineSeatReservationSystem.Data;
using AirlineSeatReservationSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace AirlineSeatReservationSystem.Controllers
{
    public class UsersController : Controller
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Kullanici.ToListAsync());
        }
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(Users model)
        {
            _context.Kullanici.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isUser = _context.Kullanici.FirstOrDefault(x => x.Email == model.Email && x.Password == model.Password);
                if (isUser != null)
                {
                    var useClaims = new List<Claim>();
                    useClaims.Add(new Claim(ClaimTypes.NameIdentifier, isUser.UserNo.ToString()));
                    useClaims.Add(new Claim(ClaimTypes.Name, isUser.UserName ?? ""));
                    useClaims.Add(new Claim(ClaimTypes.NameIdentifier, isUser.Email ?? ""));
                    if (isUser.Email == "g211210013@sakarya.edu.tr" && isUser.Password == "sau")
                    {
                        useClaims.Add(new Claim(ClaimTypes.Role, "Admin"));
                    }
                    var claimsIdentity = new ClaimsIdentity(useClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties
                    );
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Email or password is incorrect");
                }
            }

            return View(model);
        }
    }
}