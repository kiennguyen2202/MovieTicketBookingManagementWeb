using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingManagementWeb.Extensions;
using MovieTicketBookingManagementWeb.Models;

using MovieTicketBookingManagementWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace MovieTicketBookingManagementWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = SD.Role_Customer)]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Trong controller của bạn
        public async Task<IActionResult> AddToCart(int showtimeId, int seatId, List<int> popcornDrinkItemIds, List<int> popcornDrinkItemQuantitiess)
        {
            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                    .ThenInclude(r => r.Cinema)
                .FirstOrDefaultAsync(s => s.ID == showtimeId);

            var seat = await _context.Seats.FindAsync(seatId);
            var popcornDrinkItems = await _context.PopcornDrinkItems
                .Where(p => popcornDrinkItemIds.Contains(p.ID))
                .ToListAsync();

            if (showtime == null || seat == null || !popcornDrinkItems.Any())
            {
                return NotFound("Showtime, Seat, or PopcornDrinkItem not found.");
            }

            var popcornDrinkCardItems = new List<PopcornDrinkCardItem>();
            for (int i = 0; i < popcornDrinkItemIds.Count && i < popcornDrinkItemQuantitiess.Count; i++)
            {
                var popcornDrinkItem = popcornDrinkItems.Find(p => p.ID == popcornDrinkItemIds[i]);
                if (popcornDrinkItem == null)
                {
                    continue;
                }
                var popcornDrinkCardItem = new PopcornDrinkCardItem()
                {
                    ID = popcornDrinkItem.ID,
                    Name = popcornDrinkItem.Name,
                    Price = popcornDrinkItem.Price,
                    Quantity = popcornDrinkItemQuantitiess[i]
                };
                popcornDrinkCardItems.Add(popcornDrinkCardItem);
            }

            var cartItem = new CartItem
            {
                ShowtimeID = showtimeId,
                RoomID = showtime.RoomID,
                CinemaID = showtime.Room.CinemaID,
                RoomName = showtime.Room.Name,
                CinemaName = showtime.Room.Cinema.Name,
                MovieTitle = showtime.Movie.Title,
                StartTime = showtime.StartTime,
                ShowtimePrice = showtime.Price,
                SeatID = seatId,
                SeatNumber = seat.SeatNumber,
                PopcornDrinkCardItems = popcornDrinkCardItems,
                Quantity = 1 // Hoặc số lượng vé bạn muốn
            };
            // Lưu cartItem vào giỏ hàng
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            cart.Add(cartItem);
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return RedirectToAction("Index", "ShoppingCart");

        }

        public IActionResult Index()
        {
            var items = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var viewModel = new ShoppingCart
            {
                Items = items
            };

            return View(viewModel);
        }

        // Xóa một mục khỏi giỏ hàng xoa 1 dong va xoa bap nuoc 
        public IActionResult RemoveFromCart(int showtimeId, int seatId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("TicketCart");

            if (cart != null)
            {
                cart.RemoveItem(showtimeId, seatId); // Remove the item by showtimeId and seatId
                HttpContext.Session.SetObjectAsJson("TicketCart", cart);
                HttpContext.Session.SetInt32("TicketCartCount", cart.Items.Sum(item => item.Quantity));
            }

            ViewBag.TicketCartCount = cart?.Items?.Sum(item => item.Quantity) ?? 0;
            ViewBag.TicketCartTotal = cart?.Items?.Sum(item => item.TotalPrice * item.Quantity) ?? 0;

            return RedirectToAction("Index");
        }




        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("TicketCart");

            if (cart == null || !cart.Items.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(ShoppingCart cart)
        {
            var sessionCart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("TicketCart");

            if (sessionCart == null || !sessionCart.Items.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);

            foreach (var item in sessionCart.Items)
            {
                var ticket = new Ticket
                {
                    ShowtimeID = item.ShowtimeID,
                    SeatID = item.SeatID,
                    TicketType = "Standard",
                    UserID = user.Id,
                    FinalPrice = item.TotalPrice,
                    Status = "Booked",
                    BookingTime = DateTime.UtcNow,
                    MovieID = _context.Showtimes.Find(item.ShowtimeID).MovieID
                };
                _context.Tickets.Add(ticket);
            }

            await _context.SaveChangesAsync();

            // Dọn dẹp session sau khi thanh toán
            HttpContext.Session.Remove("TicketCart");
            HttpContext.Session.SetInt32("TicketCartCount", 0);

            return View("OrderCompleted", sessionCart);
        }

    }
}