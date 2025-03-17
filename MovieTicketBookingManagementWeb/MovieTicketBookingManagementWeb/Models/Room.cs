using System;
using System.Collections.Generic;

namespace MovieTicketBookingManagementWeb.Models;

public partial class Room
{
    public int Id { get; set; }

    public int CinemaId { get; set; }

    public string Name { get; set; } = null!;

    public int SeatCapacity { get; set; }

    public virtual Cinema Cinema { get; set; } = null!;

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

    public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}
