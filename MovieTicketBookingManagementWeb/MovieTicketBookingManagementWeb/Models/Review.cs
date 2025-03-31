using System;
using System.Collections.Generic;

namespace MovieTicketBookingManagementWeb.Models;

public partial class Review
{
    public int ID { get; set; }

    public string UserID { get; set; }
    

    public int MovieID { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? ReviewTime { get; set; }

    public virtual Movie? Movie { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public ApplicationUser ApplicationUser { get; set; } = null!;

}
