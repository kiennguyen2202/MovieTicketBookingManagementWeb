using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MovieTicketBookingManagementWeb.Models;

namespace MovieTicketBookingManagementWeb.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, StringLength(100)]
        public string FullName { get; set; }
        public string? Address { get; set; }
        public string? Age { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
