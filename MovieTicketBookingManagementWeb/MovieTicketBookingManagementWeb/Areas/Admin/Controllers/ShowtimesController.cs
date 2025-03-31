using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieTicketBookingManagementWeb.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketBookingManagementWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ShowtimesController : Controller
    {
        
        private readonly ApplicationDbContext _context;

        public ShowtimesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách Showtime
        public async Task<IActionResult> Index()
        {
            var showtimes = _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .Include(s => s.Room.Cinema)
                .OrderBy(s => s.StartTime);
            return View(await showtimes.ToListAsync());
        }

        [HttpGet]
        // Hiển thị form tạo mới Showtime
        public IActionResult Add()
        {
            ViewBag.MovieID = new SelectList(_context.Movies, "ID", "Title");
            ViewBag.RoomID = new SelectList(_context.Rooms, "ID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("ID,MovieID,RoomID,StartTime")] Showtime showtime)
        {
            if (ModelState.IsValid)
            {
                _context.Add(showtime);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Nếu có lỗi, giữ lại dữ liệu dropdown
            ViewBag.MovieID = new SelectList(_context.Movies, "ID", "Title", showtime.MovieID);
            ViewBag.RoomID = new SelectList(_context.Rooms, "ID", "Name", showtime.RoomID);
            return View(showtime);
        }

        [HttpGet]
        // Hiển thị form chỉnh sửa Showtime
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();

            var showtime = await _context.Showtimes.FindAsync(id);
            if (showtime == null) return NotFound();

            ViewBag.MovieID = new SelectList(_context.Movies, "ID", "Title", showtime.MovieID);
            ViewBag.RoomID = new SelectList(_context.Rooms, "ID", "Name", showtime.RoomID);
            return View(showtime);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [Bind("ID,MovieID,RoomID,StartTime")] Showtime showtime)
        {
            if (id != showtime.ID) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(showtime);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MovieID = new SelectList(_context.Movies, "ID", "Title", showtime.MovieID);
            ViewBag.RoomID = new SelectList(_context.Rooms, "ID", "Name", showtime.RoomID);
            return View(showtime);
        }
        public async Task<IActionResult> Details(int? id)
        {
           
            if (id == null) return NotFound();

            var showtime = await _context.Showtimes
                .Include(c => c.Movie)
                .Include(s => s.Room)
                .OrderBy(s => s.StartTime)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (showtime == null) return NotFound();

            return View(showtime);
        }
        // Hiển thị trang xác nhận xóa Showtime
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (showtime == null) return NotFound();

            return View(showtime);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var showtime = await _context.Showtimes.FindAsync(id);
            if (showtime != null)
            {
                _context.Showtimes.Remove(showtime);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
