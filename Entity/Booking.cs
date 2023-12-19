using System.ComponentModel.DataAnnotations;

namespace AirlineSeatReservationSystem.Entity;

        public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        public int UserNo { get; set; }
        public int FlightId { get; set; }
        public int SeatId { get; set; }
        public DateTime BookingDate { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Flight Flight { get; set; }
        public virtual Seat Seat { get; set; }
    }
