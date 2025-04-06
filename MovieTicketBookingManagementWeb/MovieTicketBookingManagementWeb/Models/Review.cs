using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MovieTicketBookingManagementWeb.Models;

public partial class Review
{
    public int ID { get; set; }
    public string UserID { get; set; }

    public int MovieID { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? ReviewTime { get; set; }

    public Movie? Movie { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public ApplicationUser User { get; set; } = null!;

}
