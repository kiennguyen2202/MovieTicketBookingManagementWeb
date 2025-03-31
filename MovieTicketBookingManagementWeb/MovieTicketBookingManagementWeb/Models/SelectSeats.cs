namespace MovieTicketBookingManagementWeb.Models
{
    public class SelectSeats
    {
        public int ID { get; set; }
        public Showtime? Showtime { get; set; }
        public List<Seat> AvailableSeats { get; set; }
    }
}
