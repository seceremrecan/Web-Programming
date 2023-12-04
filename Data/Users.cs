using System.ComponentModel.DataAnnotations;
namespace AirlineSeatReservationSystem.Data
{
    public class Users
    {
        [Key]
        public int UserNo { get; set; }
        public string? UserName { get; set; }=null!;
        public string? UserSurname { get; set; }=null!;
        public string? Phone { get; set; }=null!;
        [EmailAddress]
        public string? Email { get; set; }=null!;
        [DataType(DataType.Password)]
        public string? Password { get; set; }=null!;
    }
}