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

        
    }
}
