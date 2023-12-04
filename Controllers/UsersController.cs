using AirlineSeatReservationSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace AirlineSeatReservationSystem.Controllers
{
    public class UsersController: Controller
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
            return RedirectToAction("Index","Home");
        }
    }
}