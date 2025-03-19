using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingManagementWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace MovieTicketBookingManagementWeb.Controllers
{
    public class ShowtimesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShowtimesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int movieId)
        {
            var showtimes = await _context.Showtimes
                .Where(s => s.MovieID == movieId)
                .ToListAsync();

            return View(showtimes);
        }

        // Đặt vé
        public IActionResult Book(int showtimeId)
        {
            return View();
        }
    }
}
