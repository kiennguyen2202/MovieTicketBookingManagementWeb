using System;
using System.Collections.Generic;

namespace MovieTicketBookingManagementWeb.Models;

public partial class Cinema
{
    public int ID { get; set; }

    public string Name { get; set; } = null!;

    public string Location { get; set; } = null!;
    public string? Phone { get; set; } = null!;
    
    public string? GoogleMapEmbedUrl { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
