using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieTicketBookingManagementWeb.Models;
using MovieTicketBookingManagementWeb.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketBookingManagementWeb.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Hiển thị danh sách Review
        public async Task<IActionResult> Index()
        {
            var reviews = _context.Reviews
                .Include(r => r.Movie)
                
                .OrderBy(r => r.ReviewTime);
            return View(await reviews.ToListAsync());
        }

        [HttpGet]
        // Hiển thị form tạo mới Review
        public IActionResult Add()
        {
            ViewBag.MovieID = new SelectList(_context.Movies, "ID", "Title");
            ViewBag.UserID = new SelectList(_context.Users, "ID", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("ID,UserID,MovieID,Rating,Comment,ReviewTime")] Review review)
        {
            if (ModelState.IsValid)
            {
                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Nếu có lỗi, giữ lại dữ liệu dropdown
            ViewBag.MovieID = new SelectList(_context.Movies, "ID", "Title", review.MovieID);
           
            return View(review);
        }

        [HttpGet]
        // Hiển thị form chỉnh sửa Review
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();

            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound();

            ViewBag.MovieID = new SelectList(_context.Movies, "ID", "Title", review.MovieID);
            
            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [Bind("ID,UserID,MovieID,Rating,Comment,ReviewTime")] Review review)
        {
            if (id != review.ID) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(review);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MovieID = new SelectList(_context.Movies, "ID", "Title", review.MovieID);
           
            return View(review);
        }

        // Hiển thị chi tiết Review
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var review = await _context.Reviews
                .Include(r => r.Movie)
              
                .FirstOrDefaultAsync(m => m.ID == id);

            if (review == null) return NotFound();

            return View(review);
        }

        // Hiển thị trang xác nhận xóa Review
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var review = await _context.Reviews
                .Include(r => r.Movie)
                
                .FirstOrDefaultAsync(m => m.ID == id);
            if (review == null) return NotFound();

            return View(review);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
