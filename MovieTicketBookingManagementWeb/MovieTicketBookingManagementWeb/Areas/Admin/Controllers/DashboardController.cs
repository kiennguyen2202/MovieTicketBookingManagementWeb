using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBookingManagementWeb.Models;

namespace MovieTicketBookingManagementWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class DashboardController : Controller
    {

        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> IndexAsync()
        {
            // Thống kê tổng số người dùng đã đặt vé (khác nhau)
            var totalCustomers = await _context.Orders
                .Select(o => o.UserID)
                .Distinct()
                .CountAsync();

            // Tổng số lượng đơn hàng vé
            var totalOrders = await _context.Orders.CountAsync();

            // Tổng doanh thu từ vé
            var totalRevenue = await _context.Orders.SumAsync(o => o.TotalPrice);

            // Tổng số phim đang chiếu (có suất chiếu trong tương lai hoặc hiện tại)
           

            // Đưa dữ liệu qua view thông qua biến ViewBag
            ViewBag.TotalCustomers = totalCustomers;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalRevenue = totalRevenue;
           

            return View();
        }
        //Trả về data để vẽ chart doanh thu và số lượng vé theo ngày
        [HttpGet]
        public async Task<JsonResult> GetChartData()
        {
            var today = DateTime.Now;
            var last30Days = today.AddDays(-30); //lấy trong vòng 30 ngày

            var ordersData = await _context.Orders
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value >= last30Days && o.OrderDate.Value <= today)
                .GroupBy(o => o.OrderDate.Value.Date) // Group by theo kiểu DateTime.Date
                .Select(g => new
                {
                    Date = g.Key, // Lấy trực tiếp đối tượng DateTime
                    Orders = g.Count(),
                    Revenue = g.Sum(o => o.TotalPrice)
                })
                .OrderBy(g => g.Date)
                .ToListAsync();

            // Chuyển đổi DateTime thành chuỗi sau khi lấy dữ liệu
            var result = ordersData.Select(d => new
            {
                Date = d.Date.ToString("yyyy-MM-dd"),
                Orders = d.Orders,
                Revenue = d.Revenue
            }).ToList();

            return Json(result);
        }

        // Thống kê số lượng vé đã bán cho từng phim trong 30 ngày gần nhất
        /*
        [HttpGet]
        public async Task<JsonResult> GetMovieTicketSalesData()
        {
            var today = DateTime.Now;
            var last30Days = today.AddDays(-30);

            var ticketSalesData = await _context.OrderDetails
                .Where(od => od.Order.OrderDate >= last30Days && od.Order.OrderDate <= today && od.Showtime != null)
                .GroupBy(od => od.Showtime.Movie.Title)
                .Select(g => new
                {
                    MovieTitle = g.Key,
                    TicketsSold = g.Count()
                })
                .OrderByDescending(g => g.TicketsSold)
                .Take(10) // Lấy top 10 phim bán chạy nhất
                .ToListAsync();

            return Json(ticketSalesData);
        }
        */
    }
}