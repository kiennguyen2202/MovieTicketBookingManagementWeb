using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieTicketBookingManagementWeb.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketBookingManagementWeb.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách vé
        public async Task<IActionResult> Index()
        {
            var tickets = _context.Tickets
                .Include(t => t.Showtime)
                .Include(t => t.Seat)
                .OrderBy(t => t.BookingTime);

            return View(await tickets.ToListAsync());
        }

        // Hiển thị form tạo mới Ticket
        public IActionResult Add()
        {
            ViewBag.ShowtimeID = new SelectList(_context.Showtimes, "ID", "StartTime");
            ViewBag.SeatID = new SelectList(_context.Seats, "ID", "SeatNumber");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("ID,ShowtimeID,SeatID,TicketType,Price,Discount,FinalPrice,Status,BookingTime,PopcornQuantity,DrinkQuantity,PopcornPrice,DrinkPrice")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.FinalPrice = ticket.Price - (ticket.Discount ?? 0);
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ShowtimeID = new SelectList(_context.Showtimes, "ID", "StartTime", ticket.ShowtimeID);
            ViewBag.SeatID = new SelectList(_context.Seats, "ID", "SeatNumber", ticket.SeatID);
            return View(ticket);
        }

        // Hiển thị form chỉnh sửa Ticket
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            ViewBag.ShowtimeID = new SelectList(_context.Showtimes, "ID", "StartTime", ticket.ShowtimeID);
            ViewBag.SeatID = new SelectList(_context.Seats, "ID", "SeatNumber", ticket.SeatID);
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [Bind("ID,ShowtimeID,SeatID,TicketType,Price,Discount,FinalPrice,Status,BookingTime,PopcornQuantity,DrinkQuantity,PopcornPrice,DrinkPrice")] Ticket ticket)
        {
            if (id != ticket.ID) return NotFound();

            // Lấy thực thể Ticket hiện tại từ cơ sở dữ liệu
            var existingTicket = await _context.Tickets.FindAsync(id);

            if (existingTicket == null) return NotFound();

            if (ModelState.IsValid)
            {
                // Cập nhật các thuộc tính của thực thể đã lấy từ cơ sở dữ liệu
                existingTicket.ShowtimeID = ticket.ShowtimeID;
                existingTicket.SeatID = ticket.SeatID;
                existingTicket.TicketType = ticket.TicketType;
                existingTicket.Price = ticket.Price;
                existingTicket.Discount = ticket.Discount;
                existingTicket.FinalPrice = ticket.Price - (ticket.Discount ?? 0);
                existingTicket.Status = ticket.Status;
                existingTicket.BookingTime = ticket.BookingTime ?? existingTicket.BookingTime;  // Giữ lại BookingTime cũ nếu không có giá trị mới
                existingTicket.PopcornQuantity = ticket.PopcornQuantity;
                existingTicket.DrinkQuantity = ticket.DrinkQuantity;
                existingTicket.PopcornPrice = ticket.PopcornPrice;
                existingTicket.DrinkPrice = ticket.DrinkPrice;

                // Lưu các thay đổi
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Nếu model không hợp lệ, repopulate dropdowns
            ViewBag.ShowtimeID = new SelectList(_context.Showtimes, "ID", "StartTime", ticket.ShowtimeID);
            ViewBag.SeatID = new SelectList(_context.Seats, "ID", "SeatNumber", ticket.SeatID);

            return View(ticket);
        }


        // Hiển thị chi tiết Ticket
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Tickets
                .Include(t => t.Showtime)
                .Include(t => t.Seat)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (ticket == null) return NotFound();

            return View(ticket);
        }

        // Hiển thị trang xác nhận xóa Ticket
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Tickets
                .Include(t => t.Showtime)
                .Include(t => t.Seat)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (ticket == null) return NotFound();

            return View(ticket);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
