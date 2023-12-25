using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AirlineSeatReservationSystem.Data;
using AirlineSeatReservationSystem.Data.Concrete.Efcore;
using AirlineSeatReservationSystem.Data.Abstract;
using AirlineSeatReservationSystem.Entity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using AirlineSeatReservationSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;


namespace AirlineSeatReservationSystem.Controllers
{
    public class UsersController : Controller
    {

        private readonly IBookingRepository _bookingRepository;
        private readonly IConfiguration _configuration;

        private readonly ISeatRepository _seatRepository;

        private readonly IUserRepository _userRepository;
        private readonly IFlightRepository _flightRepository;

        public object HashHelper { get; private set; }

        public UsersController(IBookingRepository bookingRepository, IUserRepository usersRepository, IFlightRepository flightRepository, ISeatRepository seatRepository, IConfiguration configuration)
        {
            _bookingRepository = bookingRepository;

            _userRepository = usersRepository;
            _flightRepository = flightRepository;
            _configuration = configuration;

            _seatRepository = seatRepository;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _userRepository.Users.ToListAsync());
        }
        // [Authorize]
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        // [Authorize]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepository.Users.FirstOrDefaultAsync(x => x.UserName == model.UserName || x.Email == model.Email);
                if (user == null)
                {
                    var hashedPassword = _userRepository.HashPassword(model.Password);
                    _userRepository.CreateUser(new User
                    {

                        UserName = model.UserName,
                        Phone = model.Phone,
                        Email = model.Email,
                        Password = hashedPassword,

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
                return RedirectToAction("Index", "Flight");
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
                var user = await _userRepository.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
                if (user != null)
                {
                    // Admin yetkisi kontrolü
                    bool isAdmin = (user.Email == "g211210013@sakarya.edu.tr" || user.Email == "g201210093@sakarya.edu.tr")
                                   && _userRepository.VerifyPassword("sau", user.Password);

                    // Kullanıcı veya admin için şifre doğrulaması
                    if ((isAdmin && _userRepository.VerifyPassword("sau", user.Password)) ||
                        (!isAdmin && _userRepository.VerifyPassword(model.Password, user.Password)))
                    {
                        var userClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserNo.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                        if (isAdmin)
                        {
                            userClaims.Add(new Claim(ClaimTypes.Role, "admin"));
                        }

                        var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
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


                        return RedirectToAction("Index", "Flight");
                    }
                }

                ModelState.AddModelError("", "Email or password is incorrect");
            }

            return View(model);
        }

        // [HttpPost]
        // public async Task<IActionResult> SignIn(SignInViewModel model)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         var user = await _userRepository.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
        //         if (user != null)
        //         {
        //             // Admin yetkisi kontrolü
        //             bool isAdmin = (user.Email == "g211210013@sakarya.edu.tr" || user.Email == "g201210093@sakarya.edu.tr") 
        //                            && _userRepository.VerifyPassword("sau", user.Password);

        //             // Kullanıcı veya admin için şifre doğrulaması
        //             if ((isAdmin && _userRepository.VerifyPassword("sau", user.Password)) || 
        //                 (!isAdmin && _userRepository.VerifyPassword(model.Password, user.Password)))
        //             {
        //                 // JWT token oluşturma
        //                 var tokenHandler = new JwtSecurityTokenHandler();
        //                 var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
        //                 var tokenDescriptor = new SecurityTokenDescriptor
        //                 {
        //                     Subject = new ClaimsIdentity(new[]
        //                     {
        //                         new Claim(ClaimTypes.NameIdentifier, user.UserNo.ToString()),
        //                         new Claim(ClaimTypes.Name, user.UserName),
        //                         new Claim(ClaimTypes.Email, user.Email),
        //                         new Claim(ClaimTypes.Role, isAdmin ? "admin" : "user") // Rol bilgisini token'a ekleyin
        //                     }),
        //                     Expires = DateTime.UtcNow.AddHours(2),
        //                     SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //                 };

        //                 var token = tokenHandler.CreateToken(tokenDescriptor);
        //                 var tokenString = tokenHandler.WriteToken(token);

        //                 // Kullanıcıya token'ı ve rol bilgisini JSON olarak döndür
        //                 return Json(new { token = tokenString, role = isAdmin ? "admin" : "user" });
        //             }
        //             else
        //             {
        //                 ModelState.AddModelError("", "Invalid login attempt.");
        //             }
        //         }
        //         else
        //         {
        //             ModelState.AddModelError("", "User not found.");
        //         }
        //     }

        //     // Eğer model geçersizse veya giriş başarısızsa, modeli ile birlikte view'ı döndür
        //     return View(model);
        // }







    }
}