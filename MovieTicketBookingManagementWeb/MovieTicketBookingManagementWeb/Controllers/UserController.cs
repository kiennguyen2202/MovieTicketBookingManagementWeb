using Microsoft.AspNetCore.Mvc;

namespace MovieTicketBookingManagementWeb.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
