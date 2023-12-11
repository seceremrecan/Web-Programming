using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AirlineSeatReservationSystem.Models;
using AirlineSeatReservationSystem.Entity;
using AirlineSeatReservationSystem.Data.Abstract;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace AirlineSeatReservationSystem.Controllers;

public class FlightController : Controller
{
    private IFlightRepository _repository;
    public FlightController(IFlightRepository repository)
    {
        _repository = repository;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(FlightViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _repository.Flights.FirstOrDefaultAsync(x => x.From == model.From && x.To == model.To);
            if (user == null)
            {
                _repository.getFlight(new Flight
                {
                    From = model.From,
                    To = model.To,
                    Depart = model.Depart,
                    Return = model.Return,
                    Guest = model.Guest

                });

                return RedirectToAction("SignIn");
            }


        }
        return View(model);
    }

    [Authorize(Roles = "admin")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "admin")]

    public IActionResult Create(FlightCreateViewModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (ModelState.IsValid)
        {
            _repository.getFlight(
                new Flight
                {
                    From = model.From,
                    To = model.To,
                    Depart = Convert.ToString(model.Depart),
                    Return = model.Return,
                    Time = model.Time,
                    Guest = model.Guest,

                }
            );
            return RedirectToAction("Index");
        }
        return View();
    }


    public async Task<IActionResult> SearchFlights(FlightViewModel model)
    {
        if (ModelState.IsValid)
        {
            var flights = await _repository.Flights
                .Where(f => f.From == model.From && f.To == model.To
                            && f.Depart == model.Depart && f.Return == model.Return
                            && f.Guest == model.Guest)
                .Select(flightEntity => new FlightCreateViewModel
                {
                    // Örnek olarak, FlightCreateViewModel içindeki property'lere Flight entity'sinden değer atıyoruz.
                    From = flightEntity.From,
                    To = flightEntity.To,
                    Depart = flightEntity.Depart, // Flight entity'nizdeki string tipindeki Depart, FlightCreateViewModel içinde DateTime tipindeyse dönüşüm yapılır
                    Return = flightEntity.Return, // Aynı şekilde dönüşüm
                    Time = flightEntity.Time, // Eğer Time string tipindeyse direkt atama yapılır
                    Guest = flightEntity.Guest,
                    // FlightCreateViewModel içindeki diğer property'ler de burada doldurulmalı
                })
                .ToListAsync();

            return View("SearchResults", flights);
        }

        return View("Index", model);
    }


}