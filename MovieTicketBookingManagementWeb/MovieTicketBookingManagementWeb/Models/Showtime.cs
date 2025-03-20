using System;
using System.Collections.Generic;

namespace MovieTicketBookingManagementWeb.Models;

public partial class Showtime
{
    public int ID { get; set; }

    public int MovieID { get; set; }

    public int RoomID { get; set; }

    public DateTime StartTime { get; set; }

    

    public virtual Movie Movie { get; set; } = null!;

    public virtual Room Room { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
