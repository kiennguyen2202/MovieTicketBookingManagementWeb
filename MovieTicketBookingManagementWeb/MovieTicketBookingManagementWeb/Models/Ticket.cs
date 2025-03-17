using System;
using System.Collections.Generic;

namespace MovieTicketBookingManagementWeb.Models;

public partial class Ticket
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ShowtimeId { get; set; }

    public int SeatId { get; set; }

    public string TicketType { get; set; } = null!;

    public decimal Price { get; set; }

    public decimal? Discount { get; set; }

    public decimal FinalPrice { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? BookingTime { get; set; }

    public int? PopcornQuantity { get; set; }

    public int? DrinkQuantity { get; set; }

    public decimal? PopcornPrice { get; set; }

    public decimal? DrinkPrice { get; set; }

    public int? PaymentId { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual Seat Seat { get; set; } = null!;

    public virtual Showtime Showtime { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
