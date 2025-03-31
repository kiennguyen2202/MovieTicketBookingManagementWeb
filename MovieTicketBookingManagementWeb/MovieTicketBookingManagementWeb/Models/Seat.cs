using System;
using System.Collections.Generic;

namespace MovieTicketBookingManagementWeb.Models;

public partial class Seat
{
    public int ID { get; set; }

    public int RoomID { get; set; }

    public string SeatNumber { get; set; } = null!;

    public string SeatType { get; set; } = null!;
    public bool IsBooked { get; set; } = false;

    public virtual Room? Room { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
