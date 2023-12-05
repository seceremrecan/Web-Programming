using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AirlineSeatReservationSystem.Data;
using AirlineSeatReservationSystem.Data.Concrete.Efcore;
using AirlineSeatReservationSystem.Data.Abstract;
using AirlineSeatReservationSystem.Entity;

using AirlineSeatReservationSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace AirlineSeatReservationSystem.Controllers
{
    public class UsersController : Controller
    {
        private IUsersRepository _userRepository;
        public UsersController(IUsersRepository usersRepository)
        {
            _userRepository = usersRepository;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _userRepository.Users.ToListAsync());
        }
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepository.Users.FirstOrDefaultAsync(x => x.UserName == model.UserName || x.Email == model.Email);
                if (user == null)
                {
                    _userRepository.CreateUser(new Users
                    {
                        UserName = model.UserName,
                        Name = model.Name,
                        Email = model.Email,
                        Phone = model.Phone,
                        Password = model.Password

                    });
                    
                    return RedirectToAction("SignIn");
                }
                else
                {
                    ModelState.AddModelError("", "Email and password already exist");
                }

            }
            return View(model);

        }
        public IActionResult SignIn()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("SignIn");
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isUser = _userRepository.Users.FirstOrDefault(x => x.Email == model.Email && x.Password == model.Password);
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