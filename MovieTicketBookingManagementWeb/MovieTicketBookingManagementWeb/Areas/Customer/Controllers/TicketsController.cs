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
                .Include(t => t.Movie)
                //.Include(t => t.)
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
                .Include(s => s.Room.Cinema)
                .FirstOrDefaultAsync(s => s.ID == showtimeId);

            if (showtime == null) return NotFound();

            var allSeatsInRoom = _context.Seats
                .Where(s => s.RoomID == showtime.RoomID)
                .ToList();

            var bookedSeatIds = _context.Tickets
                .Where(t => t.ShowtimeID == showtimeId)
                .Select(t => t.SeatID)
                .ToList();

            // Gán trạng thái IsBooked cho từng ghế
            foreach (var seat in allSeatsInRoom)
            {
                seat.IsBooked = bookedSeatIds.Contains(seat.ID);
            }

            var viewModel = new SelectSeats
            {
                Showtime = showtime,
                AvailableSeats = allSeatsInRoom, // Sử dụng tất cả ghế trong phòng
                SeatColumns = 8 //So lượng cột ghế
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Selected(int showtimeId, int selectedSeatId)
        {
            // Lấy danh sách PopcornDrinkItem từ cơ sở dữ liệu
            var items = await _context.PopcornDrinkItems.ToListAsync();

            // Truyền showtimeId và selectedSeatId sang view
            ViewData["ShowtimeID"] = showtimeId;
            ViewData["SelectedSeatID"] = selectedSeatId;

            return View("Selected", items); 
        }

        // Xử lý đặt vé
        [HttpPost]
        public async Task<IActionResult> BookTicket(int showtimeId, int seatId, int popcorndrinkitemId)
        {
            var showtime = await _context.Showtimes.FindAsync(showtimeId);
            var popcorndrink = await _context.PopcornDrinkItems.FindAsync(popcorndrinkitemId); // Lấy PopcornDrinkItem

            if (showtime == null || popcorndrink == null)
            {
                return BadRequest("Showtime hoặc PopcornDrinkItem không tồn tại.");
            }

            decimal totalPrice = showtime.Price + popcorndrink.Price; // Tính tổng giá

            var existingTicket = await _context.Tickets.FirstOrDefaultAsync(t => t.SeatID == seatId && t.ShowtimeID == showtimeId && t.PopcornDrinkItemID == popcorndrinkitemId);
            if (existingTicket != null)
            {
                return BadRequest("Ghế này đã được đặt.");
            }

            var ticket = new Ticket
            {
                ShowtimeID = showtimeId,
                SeatID = seatId,
                TicketType = "Standard",
                PopcornDrinkItemID = popcorndrinkitemId,
                MovieID = showtime.MovieID,
                Discount = 0,
                FinalPrice = totalPrice, // Gán tổng giá
                Status = "Booked",
                BookingTime = DateTime.Now
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return RedirectToAction("TicketConfirmation", new { ticketId = ticket.ID });
        }
        [HttpGet]
        public IActionResult TicketConfirmation(int ticketId)
        {
            var ticket = _context.Tickets
    .Include(t => t.Showtime)
        .ThenInclude(s => s.Movie)
    .Include(t => t.Showtime)
        .ThenInclude(s => s.Room) // Tải Room thông qua Showtime
    .Include(t => t.Seat)
    .Include(t => t.PopcornDrinkItem)
    
    .Include(t => t.OrderDetails)
        .ThenInclude(od => od.PopcornDrinkItem)
            
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
