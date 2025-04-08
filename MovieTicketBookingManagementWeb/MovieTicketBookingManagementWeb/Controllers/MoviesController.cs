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


        // Xem chi tiết phim
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

                .FirstOrDefaultAsync(m => m.ID == id);

            if (movie == null)
            {
                return NotFound();
            }
            if (movie.Showtimes.Any())
            {
                ViewBag.Price = movie.Showtimes.First().Price; // Lấy giá từ Showtime đầu tiên
            }
            else
            {
                ViewBag.Price = "Chưa có giá"; // Hoặc giá mặc định nếu không có Showtime
            }

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
        [HttpPost]
        [Authorize]
        /*
        public async Task<IActionResult> AddReview(Review review)
        {
            review.UserID = _userManager.GetUserId(User);
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = review.MovieID });
        }
        */
        public async Task<IActionResult> ByGenre(string genreID)
        {
            if (string.IsNullOrEmpty(genreID))
            {
                return RedirectToAction("Index");
            }

            // Chuyển genreID từ string sang int
            if (!int.TryParse(genreID, out int genreIdInt))
            {
                return BadRequest("Invalid genre ID");
            }

            // Lấy danh sách phim theo thể loại được chọn
            var movies = await _context.Movies
                                       .Where(m => m.Genre.ID == genreIdInt) 
                                       .ToListAsync();

            // Lấy danh sách thể loại từ bảng Genres
            ViewData["Genres"] = await _context.Genres.ToListAsync();

            // Lưu thể loại hiện tại vào ViewData
            ViewData["SelectedGenre"] = genreIdInt; // Lưu kiểu int thay vì string

            return View("Index", movies);
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
