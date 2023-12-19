using AirlineSeatReservationSystem.Entity;
using AirlineSeatReservationSystem.Data.Abstract;
using AirlineSeatReservationSystem.Data.Concrete.Efcore;
using Microsoft.EntityFrameworkCore;

namespace AirlineSeatReservationSystem.Data.Concrete
{
    public class EfBookingRepository : IBookingRepository
    {

        private readonly DataContext context;

        public EfBookingRepository(DataContext ctx)
        {
            context = ctx;
        }

        public IQueryable<Booking> Bookings => context.Bookings;

        public IEnumerable<Booking> GetBookingsByUserId(int userId)
        {
            return context.Bookings
                          .Where(b => b.UserNo == userId)
                          .Include(b => b.Flight) // Eğer Flight bilgilerine ihtiyacınız varsa
                          .Include(b => b.Seat)   // Eğer Seat bilgilerine ihtiyacınız varsa
                          .ToList();
        }

    }
}