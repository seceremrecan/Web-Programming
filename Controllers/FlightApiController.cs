using AirlineSeatReservationSystem.Data.Abstract;
using AirlineSeatReservationSystem.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AirlineSeatReservationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class FlightApiController : ControllerBase
    {
        private readonly IFlightRepository _repository;

        public FlightApiController(IFlightRepository repository)
        {
            _repository = repository;
        }

        [HttpPut("{id}")]
        
        public async Task<IActionResult> Edit(int id, [FromBody] Flight flight)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var flightToUpdate = await _repository.Flights.FirstOrDefaultAsync(f => f.FlightId == id);
            if (flightToUpdate == null)
            {
                return NotFound();
            }

            // Güncelleme işlemleri
            flightToUpdate.From = flight.From;
            flightToUpdate.To = flight.To;
            flightToUpdate.Depart = flight.Depart;
            flightToUpdate.Return = flight.Return;
            flightToUpdate.Guest = flight.Guest;
            // Diğer alanlar da burada güncellenebilir

            _repository.editFlight(flightToUpdate);

            return NoContent();
        }

        [HttpDelete("{id}")]
        
        public async Task<IActionResult> Delete(int id)
        {
            var flightToDelete = await _repository.Flights.FirstOrDefaultAsync(f => f.FlightId == id);
            if (flightToDelete == null)
            {
                return NotFound();
            }

            await _repository.DeleteFlight(flightToDelete);
            return NoContent();
        }
    }
}
