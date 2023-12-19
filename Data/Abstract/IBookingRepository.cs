using AirlineSeatReservationSystem.Entity;
namespace  AirlineSeatReservationSystem.Data.Abstract
{
    public interface IBookingRepository
    {
        IQueryable<Booking> Bookings {get;}

            IEnumerable<Booking> GetBookingsByUserId(int userId);


    }
}