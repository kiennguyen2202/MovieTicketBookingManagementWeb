using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBookingManagementWeb.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketBookingManagementWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = SD.Role_Customer)]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> MyTickets()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var orders = await _context.Orders
                .Where(o => o.UserID == user.Id)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Movie)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Seat)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Showtime)
                        .ThenInclude(st => st.Room)
                            .ThenInclude(r => r.Cinema)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.PopcornDrinkItem)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }
    }   
}