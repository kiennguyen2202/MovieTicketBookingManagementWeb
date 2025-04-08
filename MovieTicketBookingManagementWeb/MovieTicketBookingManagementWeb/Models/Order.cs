using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MovieTicketBookingManagementWeb.Models;

namespace MovieTicketBookingManagementWeb.Models;

public partial class Order
{
    public int ID { get; set; }
    public string UserID { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal TotalPrice { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [ValidateNever]
    public virtual ApplicationUser User { get; set; } = null!;

    public List<CartItem> CartItems { get; set; } = new List<CartItem>();
}
