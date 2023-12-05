using Microsoft.EntityFrameworkCore;
using AirlineSeatReservationSystem.Entity;

namespace AirlineSeatReservationSystem.Data.Concrete.Efcore
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<Users> Kullanici => Set<Users>();
    }
}