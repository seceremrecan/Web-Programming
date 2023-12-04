using Microsoft.EntityFrameworkCore;

namespace AirlineSeatReservationSystem.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<Users> Kullanici => Set<Users>();
    }
}