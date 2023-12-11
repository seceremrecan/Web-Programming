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
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            var flights = await _repository.Flights
                .Where(f => f.From == model.From && f.To == model.To
                            && f.Depart == model.Depart && f.Return == model.Return
                            && f.Guest == model.Guest)
                .Select(flightEntity => new FlightCreateViewModel
                {

                    From = flightEntity.From,
                    To = flightEntity.To,
                    Depart = flightEntity.Depart,
                    Return = flightEntity.Return,
                    Time = flightEntity.Time,

                    Guest = flightEntity.Guest,

                })
                .ToListAsync();

            return View("SearchResults", flights);
        }

        return View("Index", model);
    }
    [Authorize(Roles = "admin")]
    public IActionResult Edit()
    {
        // if(id==null)
        // {
        //     return NotFound();
        // }
        // var flight=_repository.Flights.FirstOrDefault(i=> i.FlightId==id);
        // if(flight==null)
        // {
        //     return NotFound();
        // }
        // return View(new FlightCreateViewModel
        // {
        //     FlightId=flight.FlightId,
        //     From=flight.From,
        //     To=flight.To,
        //     Time=flight.Time
        // });
        return View();
    }
    [Authorize(Roles = "admin")]
    [HttpPost]
    public IActionResult Edit(FlightCreateViewModel model)
    {
       if(ModelState.IsValid)
       {
        var entityToUpdate=new Flight{
            From=model.From,
            To=model.To,
            Time=model.Time
        };
        _repository.editFlight(entityToUpdate);
        return RedirectToAction("SearchResults");
       }
       return View(model);
    }

}