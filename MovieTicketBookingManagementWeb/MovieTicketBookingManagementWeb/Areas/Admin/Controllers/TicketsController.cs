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
                ticket.FinalPrice = ticket.Showtime.Price - (ticket.Discount ?? 0);
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
                existingTicket.Showtime.Price = ticket.Showtime.Price;
                existingTicket.Discount = ticket.Discount;
                existingTicket.FinalPrice = ticket.Showtime.Price - (ticket.Discount ?? 0);
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
        // Hiển thị giao diện chọn ghế
        public async Task<IActionResult> SelectSeats(int showtimeId)
        {
            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .Include(s => s.Room.Cinema)
                .FirstOrDefaultAsync(s => s.ID == showtimeId);

            if (showtime == null) return NotFound();

            var availableSeats = _context.Seats
                .Where(s => s.RoomID == showtime.RoomID && !_context.Tickets.Any(t => t.SeatID == s.ID && t.ShowtimeID == showtimeId))
                .ToList();

            var viewModel = new SelectSeats
            {
                Showtime = showtime,
                AvailableSeats = availableSeats
            };

            return View(viewModel);
        }


        // Xử lý đặt vé
        [HttpPost]
        
        public async Task<IActionResult> BookTicket(int showtimeId, int seatId)
        {
            var showtime = await _context.Showtimes
                .Include(s => s.Room)
                .Include(s => s.Movie)
                .FirstOrDefaultAsync(s => s.ID == showtimeId);

            if (showtime == null) return NotFound("Suất chiếu không tồn tại!");

            var seat = await _context.Seats
                .FirstOrDefaultAsync(s => s.ID == seatId && s.RoomID == showtime.RoomID);

            if (seat == null) return BadRequest("Ghế không hợp lệ hoặc không thuộc phòng chiếu của suất chiếu này!");

            var existingTicket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.SeatID == seatId && t.ShowtimeID == showtimeId);

            if (existingTicket != null) return BadRequest("Ghế này đã được đặt!");

            seat.IsBooked = true;


            var ticket = new Ticket
            {
                ShowtimeID = showtimeId,
                SeatID = seatId,
                TicketType = "Standard",
                MovieID = showtime.MovieID,
                Discount = 0,
                FinalPrice = showtime.Price,
                Status = "Booked",
                BookingTime = DateTime.Now,
                
   
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return RedirectToAction("TicketConfirmation", new { ticketId = ticket.ID });
        }



        // Hiển thị thông tin vé sau khi đặt thành công
        public IActionResult TicketConfirmation(int ticketId)
        {
            var ticket = _context.Tickets
        .Include(t => t.Showtime)
        .ThenInclude(s => s.Movie)
        .Include(t => t.Seat)
        .FirstOrDefault(t => t.ID == ticketId);
        if (ticket == null) return NotFound();

            return View(ticket);
        }
        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.ID == id);
        }
        private void PopulateSelectLists(int? showtimeId = null, int? seatId = null)
        {
            ViewBag.ShowtimeID = new SelectList(_context.Showtimes, "ID", "StartTime", showtimeId);
            ViewBag.SeatID = new SelectList(_context.Seats, "ID", "SeatNumber", seatId);
        }
    }
}
