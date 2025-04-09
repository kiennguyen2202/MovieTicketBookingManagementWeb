using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingManagementWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace MovieTicketBookingManagementWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = SD.Role_Customer)]
    public class MoviesController : Controller
    {
        
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MoviesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Hiển thị danh sách phim
        public async Task<IActionResult> Index(string? searchQuery)
        {
            var movies = _context.Movies.Include(m => m.Genre).AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                movies = movies.Where(m => m.Title.Contains(searchQuery) || m.Description.Contains(searchQuery));
            }
            
            ViewData["SearchQuery"] = searchQuery;
            return View(await movies.ToListAsync()); // Đảm bảo trả về danh sách phim
        }
        [HttpGet]

        // Xem chi tiết phim
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.Genre)
                .Include(m => m.Showtimes)
                    .ThenInclude(s => s.Room)
                        .ThenInclude(r => r.Cinema)
                .Include(m => m.Reviews)
                .FirstOrDefaultAsync(m => m.ID == id);
             // Lấy danh sách đánh giá và thông tin người dùng
                        var reviews = await _context.Reviews
                            .Where(r => r.MovieID == id)
                            .Include(r => r.User)
                            .OrderByDescending(r => r.ReviewTime)
                            .ToListAsync();
            if (movie == null)
            {
                return NotFound();
            }

            // Lấy giá vé từ Showtime đầu tiên (nếu có)
            if (movie.Showtimes.Any())
            {
                ViewBag.Price = movie.Showtimes.First().Price;
            }
            else
            {
                ViewBag.Price = "Chưa có giá";
            }



           

            ViewData["Reviews"] = reviews;


            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview([FromBody] Review input)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var review = new Review
            {
                MovieID = input.MovieID,
                UserID = userId,
                Rating = input.Rating,
                Comment = input.Comment,
                ReviewTime = DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var reviews = await _context.Reviews
                .Where(r => r.MovieID == input.MovieID)
                .Include(r => r.User)
                .OrderByDescending(r => r.ReviewTime)
                .ToListAsync();

            return Json(new
            {
                success = true,
                reviews = reviews.Select(r => new {
                    userName = r.User.FullName,
                    rating = r.Rating,
                    comment = r.Comment,
                    reviewTime = r.ReviewTime?.ToString("HH:mm dd/MM/yyyy")
                })
            });

        }



        public IActionResult Trailer(int id)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.ID == id);
            if (movie == null || string.IsNullOrEmpty(movie.TrailerID))
            {
                return NotFound();
            }
            return View(movie);
        }
        [AllowAnonymous]
        public async Task<IActionResult> ByGenre(int genreId)
        {
            if (genreId <= 0)
            {
                return RedirectToAction("Index"); // Chuyển hướng đến trang chủ nếu genreId không hợp lệ
            }

            // Lấy danh sách phim theo genreId từ cơ sở dữ liệu
            var movies = await _context.Movies
                                       .Where(m => m.GenreID == genreId) // Lọc theo GenreID
                                       .ToListAsync();

            // Lấy tên của genre từ cơ sở dữ liệu (hoặc từ bảng Genre nếu có)
            var genre = await _context.Genres
                                      .FirstOrDefaultAsync(g => g.ID == genreId);

            // Lưu tên genre vào ViewData để hiển thị trên view
            ViewData["GenreName"] = genre?.Name ?? "Unknown Genre";

            return View(movies); // Trả về danh sách phim đã lọc theo genre
        }
        [AllowAnonymous]
        public IActionResult GetShowtimesByDate(int movieId, DateTime date)
        {
            var showtimes = _context.Showtimes
                .Where(s => s.MovieID == movieId && s.StartTime.Date == date.Date)
                .Select(s => new
                {
                    s.ID,
                    StartTime = s.StartTime.ToString("HH:mm"), // Lấy giờ theo định dạng HH:mm
                    RoomName = s.Room.Name,
                    CinemaName = s.Room.Cinema.Name,
                    cinemaAddress = s.Room.Cinema.Location,
                    s.Price
                })
                .ToList()
                .OrderBy(s => s.StartTime); // Sắp xếp theo giờ bắt đầu

            return Json(showtimes);
        }
       





    }
}
