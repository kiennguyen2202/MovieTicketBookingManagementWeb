using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingManagementWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MovieTicketBookingManagementWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách phim
        public async Task<IActionResult> Index(string? searchQuery, string? genre)
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
        public async Task<IActionResult> Add([Bind("ID,Title,Language,Duration,ReleaseDate,Description,TrailerID,GenreID")] Movie movie, IFormFile PosterUrl)
        {
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
        public async Task<IActionResult> Update(int id, [Bind("ID,Title,Language,Duration,ReleaseDate,Description,TrailerID,GenreID")] Movie movie, IFormFile PosterUrl)
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
        /*
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
        */
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
        }*/

        public async Task<IActionResult> ByGenre(int genreId)
        {
            if (genreId == 0)
            {
                return RedirectToAction("Index");
            }

            // Lấy danh sách phim theo thể loại (GenreId)
            var movies = await _context.Movies
                                       .Where(m => m.GenreID == genreId) // Kiểm tra GenreId
                                       .ToListAsync();

            // Lấy tên thể loại từ database
            var genre = await _context.Genres.FindAsync(genreId);
            ViewData["GenreName"] = genre?.Name ?? "Thể loại không xác định";

            return View("Index", movies);
        }



    }
}
