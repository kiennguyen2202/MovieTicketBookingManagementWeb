using System;
using System.Collections.Generic;

namespace MovieTicketBookingManagementWeb.Models;

public partial class Movie
{
    public int ID { get; set; }

    public string Title { get; set; } = null!;

    public int GenreID { get; set; }
    public string Language { get; set; } = null!;

    public int Duration { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public string? Description { get; set; }

    public string? PosterUrl { get; set; }
    public string? TrailerID { get; set; }
    
    
    public virtual Genre? Genre { get; set; }
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}
