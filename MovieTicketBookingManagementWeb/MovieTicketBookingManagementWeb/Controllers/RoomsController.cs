using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieTicketBookingManagementWeb.Models; // Import Models
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketBookingManagementWeb.Controllers
{
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách Room
        public async Task<IActionResult> Index()
        {
            var rooms = _context.Rooms.Include(r => r.Cinema);
            return View(await rooms.ToListAsync());
        }
        [HttpGet]
        // Hiển thị form tạo mới Room
        public IActionResult Add()
        {

            ViewBag.CinemaID = new SelectList(_context.Cinemas, "ID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("ID,Name,SeatCapacity,CinemaID")] Room room)
        {

           if (ModelState.IsValid)
        {
            _context.Add(room);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.CinemaID = new SelectList(_context.Cinemas, "ID", "Name", room.CinemaID);
        return View(room);
        }
        [HttpGet]
        // Hiển thị form chỉnh sửa Room
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            ViewData["CinemaID"] = new SelectList(_context.Cinemas, "ID", "Name", room.CinemaID);
            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [Bind("ID,Name,SeatCapacity,CinemaID")] Room room)
        {
            if (id != room.ID) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CinemaID"] = new SelectList(_context.Cinemas, "ID", "Name", room.CinemaID);
            return View(room);
        }

        // Xóa Room
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms
                .Include(r => r.Cinema)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (room == null) return NotFound();

            return View(room);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
