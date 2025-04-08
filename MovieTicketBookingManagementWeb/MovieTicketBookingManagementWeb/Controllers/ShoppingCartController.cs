using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingManagementWeb.Extensions;
using MovieTicketBookingManagementWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace MovieTicketBookingManagementWeb.Controllers
{

    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> AddToCart(int showtimeId, int seatId, List<int> selectedPopcornDrinkItemIds, List<int> popcornDrinkItemIds, List<int> popcornDrinkItemQuantitiess)
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

            if (showtime == null || seat == null)
                return NotFound("Showtime or Seat not found.");

            var popcornDrinkCardItems = new List<PopcornDrinkCardItem>();
            for (int i = 0; i < popcornDrinkItemIds.Count && i < popcornDrinkItemQuantitiess.Count; i++)
            {
                if (!selectedPopcornDrinkItemIds.Contains(popcornDrinkItemIds[i]))
                    continue;

                var item = popcornDrinkItems.FirstOrDefault(p => p.ID == popcornDrinkItemIds[i]);
                if (item == null) continue;

                popcornDrinkCardItems.Add(new PopcornDrinkCardItem
                {
                    ID = item.ID,
                    Name = item.Name,
                    Price = item.Price,
                    Quantity = popcornDrinkItemQuantitiess[i]
                });
            }

            var user = await _userManager.GetUserAsync(User);
            var sessionKey = $"TicketCart_{user.Id}";
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>(sessionKey) ?? new ShoppingCart();

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
                Quantity = 1
            };

            var existingItem = cart.Items.FirstOrDefault(i => i.ShowtimeID == showtimeId && i.SeatID == seatId);
            if (existingItem != null)
                existingItem.Quantity += 1;
            else
                cart.AddItem(cartItem);

            HttpContext.Session.SetObjectAsJson(sessionKey, cart);
            HttpContext.Session.SetInt32("TicketCartCount", cart.Items.Sum(i => i.Quantity));

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var sessionKey = $"TicketCart_{user.Id}";
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>(sessionKey) ?? new ShoppingCart();

            ViewBag.TicketCartCount = cart.Items.Sum(i => i.Quantity);
            return View(cart);
        }

        public async Task<IActionResult> RemoveFromCart(int showtimeId, int seatId)
        {
            var user = await _userManager.GetUserAsync(User);
            var sessionKey = $"TicketCart_{user.Id}";
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>(sessionKey);

            if (cart != null)
            {
                cart.RemoveItem(showtimeId, seatId);
                HttpContext.Session.SetObjectAsJson(sessionKey, cart);
                HttpContext.Session.SetInt32("TicketCartCount", cart.Items.Sum(i => i.Quantity));
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            var sessionKey = $"TicketCart_{user.Id}";
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>(sessionKey);

            if (cart == null || !cart.Items.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            var orderDetails = cart.Items.Select(item =>
            {
                var movieId = _context.Showtimes.FirstOrDefault(s => s.ID == item.ShowtimeID)?.MovieID ?? 0;

                return new OrderDetail
                {
                    ShowtimeID = item.ShowtimeID,
                    SeatID = item.SeatID,
                    RoomID = item.RoomID,
                    MovieID = movieId,
                    Quantity = item.Quantity,
                    Price = item.ShowtimePrice,
                    PopcornDrinkItemID = item.PopcornDrinkCardItems.FirstOrDefault()?.ID ?? 0
                };
            }).ToList();

            var order = new Order
            {
                OrderDetails = orderDetails,
                CartItems = cart.Items
            };

            return View("Checkout", order);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            var user = await _userManager.GetUserAsync(User);
            var sessionKey = $"TicketCart_{user.Id}";
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>(sessionKey);

            if (cart == null || !cart.Items.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            var finalOrder = new Order
            {
                User = user,
                OrderDate = DateTime.UtcNow,
                TotalPrice = 0,
                OrderDetails = new List<OrderDetail>()
            };

            decimal totalPrice = 0;

            foreach (var item in cart.Items)
            {
                var movieId = _context.Showtimes.FirstOrDefault(s => s.ID == item.ShowtimeID)?.MovieID ?? 0;

                var ticket = new Ticket
                {
                    ShowtimeID = item.ShowtimeID,
                    SeatID = item.SeatID,
                    MovieID = movieId,
                    TicketType = "Standard",
                    UserID = user.Id,
                    BookingTime = DateTime.UtcNow,
                    Status = "Booked",
                    FinalPrice = item.TotalPrice,
                    PopcornDrinkItemID = item.PopcornDrinkCardItems.FirstOrDefault()?.ID ?? 0,
                };

                _context.Tickets.Add(ticket);

                finalOrder.OrderDetails.Add(new OrderDetail
                {
                    ShowtimeID = item.ShowtimeID,
                    SeatID = item.SeatID,
                    RoomID = item.RoomID,
                    MovieID = movieId,
                    Ticket = ticket,
                    Quantity = 1,
                    Price = item.ShowtimePrice,
                    PopcornDrinkItemID = null,
                    Order = finalOrder
                });

                foreach (var popcorn in item.PopcornDrinkCardItems)
                {
                    finalOrder.OrderDetails.Add(new OrderDetail
                    {
                        ShowtimeID = item.ShowtimeID,
                        SeatID = item.SeatID,
                        RoomID = item.RoomID,
                        MovieID = movieId,
                        Ticket = ticket,
                        Quantity = popcorn.Quantity,
                        Price = popcorn.Price,
                        PopcornDrinkItemID = popcorn.ID,
                        Order = finalOrder
                    });

                    totalPrice += popcorn.Quantity * popcorn.Price;
                }

                totalPrice += item.TotalPrice;
            }

            finalOrder.TotalPrice = totalPrice;

            _context.Orders.Add(finalOrder);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove(sessionKey);
            HttpContext.Session.SetInt32("TicketCartCount", 0);

            return RedirectToAction("OrderCompleted", new { orderId = finalOrder.ID });
        }

        public async Task<IActionResult> OrderCompleted(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails).ThenInclude(od => od.Movie)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Seat)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Showtime)
                .Include(o => o.OrderDetails).ThenInclude(od => od.PopcornDrinkItem)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.ID == orderId);

            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Index");
            }

            return View(order);
        }
    }
}