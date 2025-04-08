using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBookingManagementWeb.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketBookingManagementWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách đơn hàng
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.User) // Liên kết với người dùng đặt hàng
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Showtime)
                        .ThenInclude(s => s.Movie) // Liên kết với phim
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Seat) // Liên kết với ghế
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.PopcornDrinkItem) // Liên kết với bắp nước (nếu có)
                .ToListAsync();

            return View(orders);
        }

        // Xem chi tiết đơn hàng
        public async Task<IActionResult> Detail(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User) // Liên kết với người dùng đặt hàng
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Showtime)
                        .ThenInclude(s => s.Movie) // Liên kết với phim
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Seat) // Liên kết với ghế
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.PopcornDrinkItem) // Liên kết với bắp nước (nếu có)
                .FirstOrDefaultAsync(o => o.ID == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // Hiển thị form tạo mới đơn hàng (có thể không cần thiết cho admin, đơn hàng thường được tạo tự động)
        public IActionResult Add()
        {
            // Có thể bạn sẽ muốn hiển thị danh sách người dùng, suất chiếu, ghế, bắp nước để tạo đơn hàng thủ công
            ViewBag.Users = _context.Users.ToList();
            ViewBag.Showtimes = _context.Showtimes.Include(s => s.Movie).Include(s => s.Room).ToList();
            ViewBag.Seats = _context.Seats.Include(s => s.Room).ToList();
            ViewBag.PopcornDrinkItems = _context.PopcornDrinkItems.ToList();
            return View();
        }

        // Xử lý tạo mới đơn hàng (có thể không cần thiết cho admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Users = _context.Users.ToList();
            ViewBag.Showtimes = _context.Showtimes.Include(s => s.Movie).Include(s => s.Room).ToList();
            ViewBag.Seats = _context.Seats.Include(s => s.Room).ToList();
            ViewBag.PopcornDrinkItems = _context.PopcornDrinkItems.ToList();
            return View(order);
        }

        // Hiển thị form chỉnh sửa đơn hàng
        public async Task<IActionResult> Update(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewBag.Users = _context.Users.ToList();
            return View(order);
        }

        // Xử lý chỉnh sửa đơn hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Order order)
        {
            if (id != order.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Users = _context.Users.ToList();
            return View(order);
        }

        // Xác nhận xóa đơn hàng
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // Xử lý xóa đơn hàng
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}