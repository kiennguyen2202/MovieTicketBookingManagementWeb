using System;
using System.Collections.Generic;

namespace MovieTicketBookingManagementWeb.Models;

public partial class OrderDetail
{
    public int ID { get; set; }
    public int ShowtimeID { get; set; }
    public int SeatID { get; set; }
    public int RoomID { get; set; }
    public int MovieID { get; set; }

  
    public int OrderID { get; set; }

    public int TicketID { get; set; }

    public int Quantity { get; set; }

    public decimal? Price { get; set; }
    public int? PopcornDrinkItemID { get; set; }
    public PopcornDrinkItem? PopcornDrinkItem { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Ticket Ticket { get; set; } = null!;

    public virtual Showtime? Showtime { get; set; } = null!;
    public virtual Seat? Seat { get; set; } = null!;
    public virtual Room Room { get; set; } = null!;
    public virtual Movie? Movie { get; set; } = null!;

}
