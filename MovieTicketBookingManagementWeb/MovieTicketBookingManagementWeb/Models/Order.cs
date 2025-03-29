using System;
using System.Collections.Generic;
using MovieTicketBookingManagementWeb.Models;

namespace MovieTicketBookingManagementWeb.Models;

public partial class Order
{
    public int ID { get; set; }

    public int UserId { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal TotalPrice { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public ApplicationUser ApplicationUser { get; set; }
}
