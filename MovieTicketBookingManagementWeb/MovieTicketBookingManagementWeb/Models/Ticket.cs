using System;
using System.Collections.Generic;

namespace MovieTicketBookingManagementWeb.Models;

public partial class Ticket
{
    public int ID { get; set; }
    public int MovieID { get; set; }

    public int ShowtimeID { get; set; }

    public int SeatID { get; set; }

    public string TicketType { get; set; } = null!;

    public int UserID { get; set; }

    public decimal? Discount { get; set; }

    public decimal FinalPrice { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? BookingTime { get; set; }

    public int? PopcornQuantity { get; set; }

    public int? DrinkQuantity { get; set; }

    public decimal? PopcornPrice { get; set; }

    public decimal? DrinkPrice { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    public virtual Movie? Movie { get; set; } = null!;

    public virtual Seat? Seat { get; set; } = null!;

    public virtual Showtime? Showtime { get; set; } = null!;
    public ApplicationUser ApplicationUser { get; set; } = null!;

}
