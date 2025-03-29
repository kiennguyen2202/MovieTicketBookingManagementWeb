using System;
using System.Collections.Generic;

namespace MovieTicketBookingManagementWeb.Models;

public partial class Payment
{
    public int ID { get; set; }

 

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public DateTime? PaymentTime { get; set; }

    public int? OrderID { get; set; }

    public virtual Order? Order { get; set; }


}
