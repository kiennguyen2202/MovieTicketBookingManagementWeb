using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingManagementWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MovieTicketBookingManagementWeb.Controllers
{
   
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

            // Lọc theo từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(searchQuery))
            {
                movies = movies.Where(m => m.Title.Contains(searchQuery) || m.Description.Contains(searchQuery));
            }
            




            ViewData["SearchQuery"] = searchQuery;
            
            return View(await movies.ToListAsync());
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.GenreID = new SelectList(_context.Genres, "ID", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add([Bind("ID,Title,Language,Duration,ReleaseDate,Description,GenreID")] Movie movie, IFormFile PosterUrl)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(movie.TrailerID))
                {
                    movie.TrailerID = "default_trailer_id"; // Hoặc một giá trị mặc định hợp lệ
                }
                if (PosterUrl != null && PosterUrl.Length > 0)
                {
                    var filePath = Path.Combine("wwwroot/images", PosterUrl.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await PosterUrl.CopyToAsync(stream);
                    }
                    movie.PosterUrl = "/images/" + PosterUrl.FileName;
                }

                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.GenreID = new SelectList(_context.Genres, "ID", "Name", movie.GenreID);
            return View(movie);
        }


        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();

            ViewBag.GenreID = new SelectList(_context.Genres, "ID", "Name", movie.GenreID);

            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, [Bind("ID,Title,Language,Duration,ReleaseDate,Description,GenreID,PosterUrl")] Movie movie, IFormFile PosterUrl)
        {
            if (id != movie.ID) return NotFound();

            if (ModelState.IsValid)
            {
                if (PosterUrl != null && PosterUrl.Length > 0)
                {
                    var filePath = Path.Combine("wwwroot/images", PosterUrl.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await PosterUrl.CopyToAsync(stream);
                    }
                    movie.PosterUrl = "/images/" + PosterUrl.FileName;
                }

                _context.Update(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.GenreID = new SelectList(_context.Genres, "ID", "Name", movie.GenreID);
            return View(movie);
        }


        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();

            return View(movie);
        }

        // Xử lý xóa phim
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
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
                    s.Price
                })
                .ToList()
                .OrderBy(s => s.StartTime); // Sắp xếp theo giờ bắt đầu

            return Json(showtimes);
        }








    }
}
