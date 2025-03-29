using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBookingManagementWeb.Extensions;
using MovieTicketBookingManagementWeb.Models;


namespace MovieTicketBookingManagementWeb.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public IActionResult AddToCart(int showtimeID, int quantity)
        {
            var showtime = _context.Showtimes.Include(s => s.Movie).FirstOrDefault(s => s.ID == showtimeID);
            if (showtime == null)
            {
                return NotFound();
            }

            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();

            var existingItem = cart.Items.FirstOrDefault(i => i.ShowtimeID == showtimeID);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ShowtimeID = showtimeID,
                   
                    Quantity = quantity,
                    Price = 100000 // Giá vé (hoặc lấy từ `Showtime`)
                });
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);
            HttpContext.Session.SetInt32("CartCount", cart.Items.Sum(item => item.Quantity));

            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            ViewBag.CartCount = cart.Items.Sum(item => item.Quantity);
            return View(cart);
        }

        public IActionResult RemoveFromCart(int showtimeID)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart != null)
            {
                cart.Items.RemoveAll(item => item.ShowtimeID == showtimeID);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
                HttpContext.Session.SetInt32("CartCount", cart.Items.Sum(item => item.Quantity));
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");

            if (cart == null || !cart.Items.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn trống!";
                return RedirectToAction("Index");
            }

            var order = new Order
            {
                OrderDetails = cart.Items.Select(i => new OrderDetail
                {
                    ShowtimeID = i.ShowtimeID,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null || !cart.Items.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn trống!";
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            //order.UserId = @ApplicationUser;
            order.OrderDate = DateTime.UtcNow;
            order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
            order.OrderDetails = cart.Items.Select(i => new OrderDetail
            {
                ShowtimeID = i.ShowtimeID,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList();

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("Cart");
            HttpContext.Session.SetInt32("CartCount", 0);

            return View("OrderCompleted", order);
        }
    }
}
