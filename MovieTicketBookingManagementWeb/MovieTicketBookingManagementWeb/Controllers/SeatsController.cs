using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieTicketBookingManagementWeb.Models; // Import Models
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketBookingManagementWeb.Controllers
{
    public class SeatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SeatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách Seat
        public async Task<IActionResult> Index()
        {
            var seats = _context.Seats.Include(r => r.Room);
            return View(seats);
        }
        [HttpGet]
        // Hiển thị form tạo mới Seat
        public IActionResult Add()
        {

            ViewBag.RoomID = new SelectList(_context.Rooms, "ID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("ID,SeatNumber,SeatType,RoomID")] Seat seat)
        {

            if (ModelState.IsValid)
            {
                _context.Add(seat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.RoomID = new SelectList(_context.Rooms, "ID", "Name", seat.RoomID);
            return View(seat);
        }
        [HttpGet]
        // Hiển thị form chỉnh sửa Seat
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();

            var seat = await _context.Seats.FindAsync(id);
            if (seat == null) return NotFound();

            ViewData["RoomID"] = new SelectList(_context.Rooms, "ID", "Name", seat.RoomID);
            return View(seat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [Bind("ID,SeatNumber,SeatType,RoomID")] Seat seat)
        {
            if (id != seat.ID) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(seat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoomID"] = new SelectList(_context.Rooms, "ID", "Name", seat.RoomID);
            return View(seat);
        }
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null) return NotFound();

            var seat = await _context.Seats
                .Include(c => c.Room)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (seat == null) return NotFound();

            return View(seat);
        }

        // Xóa Seat
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var seat = await _context.Seats
                .Include(r => r.Room)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (seat == null) return NotFound();

            return View(seat);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seat = await _context.Seats.FindAsync(id);
            if (seat != null)
            {
                _context.Seats.Remove(seat);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
