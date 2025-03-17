using System;
using System.Collections.Generic;

namespace MovieTicketBookingManagementWeb.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public DateTime? PaymentTime { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual User User { get; set; } = null!;
}
