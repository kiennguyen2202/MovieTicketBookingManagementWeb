using System;
using System.Collections.Generic;

namespace MovieTicketBookingManagementWeb.Models;

public partial class Review
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int MovieId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? ReviewTime { get; set; }

    public virtual Movie Movie { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
