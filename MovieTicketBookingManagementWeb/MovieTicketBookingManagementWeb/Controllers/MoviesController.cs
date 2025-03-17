using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingManagementWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace MovieTicketBookingManagementWeb.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách phim
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movies.ToListAsync());
        }

        // Xem chi tiết phim
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }
    }
}
