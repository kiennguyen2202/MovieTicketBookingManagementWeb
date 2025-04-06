using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MovieTicketBookingManagementWeb.Models;

public partial class Ticket
{
    public int ID { get; set; }
    public string UserID { get; set; }
    public int MovieID { get; set; }

    public int ShowtimeID { get; set; }

    public int SeatID { get; set; }

    public string TicketType { get; set; } = null!;

    public int PopcornDrinkItemID { get; set; }
    public int PopcornQuantity { get; set; }

    public decimal? Discount { get; set; }

    public decimal FinalPrice { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? BookingTime { get; set; }

    public ApplicationUser User { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    public virtual Movie? Movie { get; set; } = null!;

    public virtual Seat? Seat { get; set; } = null!;

    public virtual Showtime? Showtime { get; set; } = null!;
    public virtual PopcornDrinkItem? PopcornDrinkItem { get; set; }

}
