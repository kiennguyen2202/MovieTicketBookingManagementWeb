using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingManagementWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

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
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(Movie movie, IFormFile
        PosterUrl)
        {
            if (ModelState.IsValid)
            {
                if (movie.ReleaseDate != null)
                    {
                    var releaseDate = movie.ReleaseDate;
                    movie.ReleaseDate = releaseDate;
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
            return View(movie);
        }
        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();

            return View(movie);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, Movie movie, IFormFile PosterUrl)
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
        public async Task<IActionResult> AddReview(Review review)
        {
            review.UserID = _userManager.GetUserId(User);
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = review.MovieID });
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
