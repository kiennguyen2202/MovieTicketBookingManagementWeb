using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBookingManagementWeb.Extensions;
using MovieTicketBookingManagementWeb.Models;

namespace MovieTicketBookingManagementWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = SD.Role_Customer)]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _cartSessionPrefix = "TicketCart"; // Prefix for the session key

        public ShoppingCartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Phương thức để tạo key Session giỏ hàng riêng cho người dùng
        private async Task<string> GetCartSessionKey()
        {
            var user = await _userManager.GetUserAsync(User);
            return $"{_cartSessionPrefix}_{user?.Id}"; // Tạo key theo định dạng TicketCart_UserGuid
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
                if (!selectedPopcornDrinkItemIds.Contains(popcornDrinkItemIds[i]))
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
                Quantity = 1
            };

            var sessionKey = await GetCartSessionKey();
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>(sessionKey) ?? new ShoppingCart();

            var existingItem = cart.Items.FirstOrDefault(item => item.ShowtimeID == showtimeId && item.SeatID == seatId);
            if (existingItem != null)
            {
                existingItem.Quantity += 1;
            }
            else
            {
                cart.AddItem(cartItem);
            }

            HttpContext.Session.SetObjectAsJson(sessionKey, cart);
            HttpContext.Session.SetInt32("TicketCartCount", cart.Items.Sum(item => item.Quantity));
            ViewBag.TicketCartCount = cart.Items.Sum(item => item.Quantity);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index()
        {
            var sessionKey = await GetCartSessionKey();
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>(sessionKey) ?? new ShoppingCart();
            ViewBag.TicketCartCount = cart.Items?.Sum(item => item.Quantity) ?? 0;
            return View(cart);
        }

        public async Task<IActionResult> RemoveFromCart(int showtimeId, int seatId)
        {
            var sessionKey = await GetCartSessionKey();
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>(sessionKey);

            if (cart != null)
            {
                cart.RemoveItem(showtimeId, seatId);
                HttpContext.Session.SetObjectAsJson(sessionKey, cart);
                HttpContext.Session.SetInt32("TicketCartCount", cart.Items.Sum(item => item.Quantity));
            }

            ViewBag.TicketCartCount = cart?.Items?.Sum(item => item.Quantity) ?? 0;
            ViewBag.TicketCartTotal = cart?.Items?.Sum(item => item.TotalPrice * item.Quantity) ?? 0;

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var sessionKey = await GetCartSessionKey();
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>(sessionKey);

            if (cart == null || !cart.Items.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            var orderDetails = new List<OrderDetail>();

            foreach (var cartItem in cart.Items)
            {
                var showtime = await _context.Showtimes.FirstOrDefaultAsync(s => s.ID == cartItem.ShowtimeID);
                var movie = await _context.Movies.FirstOrDefaultAsync(m => m.ID == showtime.MovieID);
                var seat = await _context.Seats.FirstOrDefaultAsync(s => s.ID == cartItem.SeatID);

                // Tạo OrderDetail cho vé xem phim
                orderDetails.Add(new OrderDetail
                {
                    ShowtimeID = cartItem.ShowtimeID,
                    Showtime = showtime,
                    SeatID = cartItem.SeatID,
                    Seat = seat,
                    RoomID = cartItem.RoomID,
                    MovieID = movie?.ID ?? 0,
                    Movie = movie,
                    Quantity = 1, // Mỗi vé là 1
                    Price = cartItem.ShowtimePrice,
                    PopcornDrinkItemID = null, // Đánh dấu là vé xem phim
                    PopcornDrinkItem = null
                });

                // Tạo OrderDetail cho từng loại bắp nước
                foreach (var popcornDrink in cartItem.PopcornDrinkCardItems)
                {
                    var popcornDrinkItem = await _context.PopcornDrinkItems.FirstOrDefaultAsync(p => p.ID == popcornDrink.ID);
                    if (popcornDrinkItem != null)
                    {
                        orderDetails.Add(new OrderDetail
                        {
                            ShowtimeID = cartItem.ShowtimeID,
                            Showtime = showtime,
                            SeatID = cartItem.SeatID,
                            Seat = seat,
                            RoomID = cartItem.RoomID,
                            MovieID = movie?.ID ?? 0,
                            Movie = movie,
                            Quantity = popcornDrink.Quantity,
                            Price = popcornDrink.Price,
                            PopcornDrinkItemID = popcornDrink.ID,
                            PopcornDrinkItem = popcornDrinkItem
                        });
                    }
                }
            }

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
            var sessionKey = await GetCartSessionKey();
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>(sessionKey);

            if (cart == null || !cart.Items.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            order.User = user;
            decimal totalPrice = 0;
            var finalOrder = new Order
            {
                User = user,
                OrderDate = DateTime.UtcNow,
                TotalPrice = 0,
                OrderDetails = new List<OrderDetail>()
            };
            foreach (var item in cart.Items)
            {
                var movieId = _context.Showtimes.FirstOrDefault(s => s.ID == item.ShowtimeID)?.MovieID ?? 0;
                var ticket = new Ticket
                {
                    ShowtimeID = item.ShowtimeID,
                    SeatID = item.SeatID,
                    TicketType = "Standard",
                    UserID = user.Id,
                    FinalPrice = item.TotalPrice,
                    Status = "Booked",
                    BookingTime = DateTime.UtcNow,
                    MovieID = movieId,
                    PopcornDrinkItemID = item.PopcornDrinkCardItems.FirstOrDefault()?.ID ?? 0,
                };

                _context.Tickets.Add(ticket);
                var ticketOrderDetail = new OrderDetail
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
                };

                finalOrder.OrderDetails.Add(ticketOrderDetail);

                foreach (var popcornDrinkItem in item.PopcornDrinkCardItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        ShowtimeID = item.ShowtimeID,
                        SeatID = item.SeatID,
                        RoomID = item.RoomID,
                        MovieID = movieId,
                        Ticket = ticket,
                        Quantity = popcornDrinkItem.Quantity,
                        Price = popcornDrinkItem.Price,
                        PopcornDrinkItemID = popcornDrinkItem.ID,
                        Order = finalOrder,
                    };

                    finalOrder.OrderDetails.Add(orderDetail);
                    totalPrice += popcornDrinkItem.Quantity * popcornDrinkItem.Price;
                }
            }

            totalPrice += cart.Items.Sum(i => i.TotalPrice);
            finalOrder.TotalPrice = totalPrice;

            _context.Orders.Add(finalOrder);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove(sessionKey); // Xóa giỏ hàng của người dùng hiện tại sau khi thanh toán
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