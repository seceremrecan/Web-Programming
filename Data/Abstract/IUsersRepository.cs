using AirlineSeatReservationSystem.Entity;

namespace  AirlineSeatReservationSystem.Data.Abstract
{
    public interface IUsersRepository
    {
        IQueryable<Users> Users {get;}

        void CreateUser(Users user);

    }
}