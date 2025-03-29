using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBookingManagementWeb.Models;
using System.Threading.Tasks;

namespace MovieTicketBookingManagementWeb.Controllers
{
   
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách vé
        public async Task<IActionResult> Index()
        {
            var order = await _context.Orders
                //.Include(o => o.Showtime) // Liên kết suất chiếu
                //.Include(o => o.Seat) // Liên kết ghế
                //.Include(t => t.ApplicationUser) // Liên kết người dùng (nếu có)
                .ToListAsync();

            return View(order); // Trả về danh sách vé
        }

        // Hiển thị chi tiết vé
        public async Task<IActionResult> Detail(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Showtime)
                .Include(t => t.Seat)
                //.Include(t => t.ApplicationUser)
                .FirstOrDefaultAsync(t => t.ID == id);

            if (ticket == null)
            {
                return NotFound(); // Nếu không tìm thấy vé
            }

            return View(ticket); // Trả về chi tiết vé
        }

        // Hiển thị form tạo mới vé
        public IActionResult Add()
        {
            ViewBag.Users = _context.Users.ToList();
            return View();
        }

        // Xử lý tạo mới vé
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                _context.Tickets.Add(ticket); // Thêm vé vào cơ sở dữ liệu
                await _context.SaveChangesAsync(); // Lưu thay đổi
                return RedirectToAction(nameof(Index)); // Điều hướng về trang danh sách vé
            }
            return View(ticket); // Trả về view với lỗi nếu có
        }

        // Hiển thị form chỉnh sửa vé
        public async Task<IActionResult> Update(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound(); // Nếu không tìm thấy vé
            }
            return View(ticket); // Trả về view chỉnh sửa vé
        }

        // Xử lý chỉnh sửa vé
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Ticket ticket)
        {
            if (id != ticket.ID)
            {
                return NotFound(); // Nếu ID không khớp
            }

            if (ModelState.IsValid)
            {
                _context.Tickets.Update(ticket); // Cập nhật vé
                await _context.SaveChangesAsync(); // Lưu thay đổi
                return RedirectToAction(nameof(Index)); // Điều hướng về trang danh sách vé
            }
            return View(ticket); // Trả về view nếu có lỗi
        }

        // Xác nhận xóa vé
        public async Task<IActionResult> Delete(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound(); // Nếu không tìm thấy vé
            }
            return View(ticket); // Trả về view xác nhận xóa vé
        }

        // Xử lý xóa vé
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket); // Xóa vé
                await _context.SaveChangesAsync(); // Lưu thay đổi
            }
            return RedirectToAction(nameof(Index)); // Điều hướng về trang danh sách vé
        }
    }
}
