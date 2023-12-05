using AirlineSeatReservationSystem.Entity;
using AirlineSeatReservationSystem.Data.Abstract;
using AirlineSeatReservationSystem.Data.Concrete.Efcore;

namespace AirlineSeatReservationSystem.Data.Concrete
{
    public class EfUsersRepository : IUsersRepository
    {

        private DataContext _context;
        public EfUsersRepository(DataContext context)
        {
            _context = context;
        }
        public IQueryable<Users> Users => _context.Kullanici;
        public void CreateUser(Users user)
        {
            _context.Kullanici.Add(user);
            _context.SaveChanges();
        }
    }
}