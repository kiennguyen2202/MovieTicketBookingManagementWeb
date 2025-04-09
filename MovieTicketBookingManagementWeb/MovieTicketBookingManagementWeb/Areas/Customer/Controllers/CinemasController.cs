using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingManagementWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace MovieTicketBookingManagementWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = SD.Role_Customer)]
    public class CinemasController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CinemasController(ApplicationDbContext context)
        {
            _context = context;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var cinemas = await _context.Cinemas.ToListAsync();
            return View(cinemas);
        }
        
    }
}