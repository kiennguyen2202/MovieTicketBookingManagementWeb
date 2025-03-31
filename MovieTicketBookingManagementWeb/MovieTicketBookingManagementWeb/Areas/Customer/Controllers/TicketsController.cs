using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieTicketBookingManagementWeb.Models;
using System.Linq;
using System.Threading.Tasks;


namespace MovieTicketBookingManagementWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = SD.Role_Customer)]
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
        
        // Hiển thị giao diện chọn ghế
        public async Task<IActionResult> SelectSeats(int showtimeId)
        {
            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
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
        public async Task<IActionResult> BookTicket(int showtimeId, int seatId, decimal price)
        {
            var existingTicket = await _context.Tickets.FirstOrDefaultAsync(t => t.SeatID == seatId && t.ShowtimeID == showtimeId);
            if (existingTicket != null)
            {
                return BadRequest("Ghế này đã được đặt.");
            }

            var ticket = new Ticket
            {
                ShowtimeID = showtimeId,
                SeatID = seatId,
                TicketType = "Standard",
                //Showtime.Price = price,
                Discount = 0,
                FinalPrice = price,
                Status = "Booked",
                BookingTime = DateTime.Now
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return RedirectToAction("TicketConfirmation", new { ticketId = ticket.ID });
        }


        // Hiển thị thông tin vé sau khi đặt thành công
        public IActionResult TicketConfirmation(int ticketId)
        {
            var ticket = _context.Tickets.Include(t => t.Movie).Include(t => t.Seat).FirstOrDefault(t => t.ID == ticketId);
            if (ticket == null) return NotFound();

            return View(ticket);
        }
    }
}
