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
    public class MoviesController : Controller
    {
        
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách phim
        public async Task<IActionResult> Index(string? searchQuery)
        {
            var movies = _context.Movies.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                movies = movies.Where(m => m.Title.Contains(searchQuery) || m.Description.Contains(searchQuery));
            }
            ViewBag.Genres = await _context.Movies
        .Where(m => m.Genre != null)
        .Select(m => m.Genre)
        .Distinct()
        .ToListAsync();
            ViewData["SearchQuery"] = searchQuery;
            return View(await movies.ToListAsync()); // Đảm bảo trả về danh sách phim
        }
        [HttpGet]

        // Xem chi tiết phim
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                 .Include(m => m.Showtimes)

                 .ThenInclude(s => s.Room)
                 .ThenInclude(r => r.Cinema)
                 .Include(m => m.Reviews)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (movie == null)
            {
                return NotFound();
            }
            var averageRating = movie.Reviews.Any() ? movie.Reviews.Average(r => r.Rating) : 0;
            var reviews = await _context.Reviews
        .Where(r => r.MovieID == id)
        .Include(r => r.ApplicationUser) // Lấy thông tin người dùng
        .OrderByDescending(r => r.ReviewTime)
        .ToListAsync();
            // Gửi thông tin về điểm trung bình và reviews đến view
            ViewBag.AverageRating = averageRating;

            return View(movie);

        }
        public async Task<IActionResult> ByGenre(string genre)
        {
            if (string.IsNullOrEmpty(genre))
            {
                return RedirectToAction("Index");
            }

            // Lọc phim theo thể loại
            var movies = await _context.Movies
                .Where(m => m.Genre == genre)
                .ToListAsync();

            ViewData["GenreName"] = genre;

            return View("Index", movies); // Sử dụng chung View Index để hiển thị phim
        }
        public async Task<IActionResult> LoadGenres()
        {
            var genres = await _context.Movies
                .Where(m => m.Genre != null)
                .Select(m => m.Genre)
                .Distinct()
                .ToListAsync();

            ViewBag.Genres = genres;

            return PartialView("_GenreDropdown", genres); // Tạo một Partial View nếu cần
        }



    }
}
