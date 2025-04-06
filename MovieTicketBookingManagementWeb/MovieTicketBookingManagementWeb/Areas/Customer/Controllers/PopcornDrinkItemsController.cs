using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingManagementWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace MovieTicketBookingManagementWeb.Area.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = SD.Role_Customer)]
    public class PopcornDrinkItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PopcornDrinkItemsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var popcorndrinkitem = await _context.PopcornDrinkItems.ToListAsync();
            return View(popcorndrinkitem);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var popcorndrinkitem = await _context.PopcornDrinkItems

                .FirstOrDefaultAsync(m => m.ID == id);

            if (popcorndrinkitem == null) return NotFound();

            return View(popcorndrinkitem);
        }



        [HttpPost]
        public async Task<IActionResult> Confirm(int showtimeId, int selectedSeatId, List<int> selectedItems, Dictionary<string, int> quantities)
        {
            if (selectedItems == null || quantities == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            var selectedPopcornDrinks = await _context.PopcornDrinkItems
                .Where(item => selectedItems.Contains(item.ID))
                .ToListAsync();

            Dictionary<int, int> itemQuantities = new Dictionary<int, int>();
            decimal totalPopcornDrinkPrice = 0;
            int popcornDrinkItemID = 0;
            int popcornQuantity = 0;

            foreach (var item in selectedPopcornDrinks)
            {
                if (quantities.ContainsKey($"quantity_{item.ID}"))
                {
                    itemQuantities[item.ID] = quantities[$"quantity_{item.ID}"];
                    totalPopcornDrinkPrice += item.Price * quantities[$"quantity_{item.ID}"];
                    popcornDrinkItemID = item.ID;
                    popcornQuantity = quantities[$"quantity_{item.ID}"];
                }
            }

            // Tạo vé
            var showtime = await _context.Showtimes.FindAsync(showtimeId);
            decimal price = showtime.Price;

            var ticket = new Ticket
            {
                ShowtimeID = showtimeId,
                SeatID = selectedSeatId,
                TicketType = "Standard",
                MovieID = showtime.MovieID,
                Discount = 0,
                FinalPrice = price + totalPopcornDrinkPrice, // Thêm giá bắp nước vào giá vé
                Status = "Booked",
                BookingTime = DateTime.Now,
                PopcornDrinkItemID = popcornDrinkItemID,
                PopcornQuantity = popcornQuantity
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return RedirectToAction("TicketConfirmation", "Tickets", new { ticketId = ticket.ID });
        }
    }
}

