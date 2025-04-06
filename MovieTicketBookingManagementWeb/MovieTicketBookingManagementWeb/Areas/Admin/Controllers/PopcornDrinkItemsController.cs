using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingManagementWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace MovieTicketBookingManagementWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
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
        [HttpGet]
        public IActionResult Add()
        {

            return View();
        }
        [HttpPost]
        
        public async Task<IActionResult> Add(PopcornDrinkItem popcorndrinkitem,IFormFile PictureUrl)
        {
            if (ModelState.IsValid)
            {
                
                if (PictureUrl != null && PictureUrl.Length > 0)
                {
                    var filePath = Path.Combine("wwwroot/images", PictureUrl.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await PictureUrl.CopyToAsync(stream);
                    }
                    popcorndrinkitem.PictureUrl = "/images/" + PictureUrl.FileName;
                }
                _context.PopcornDrinkItems.Add(popcorndrinkitem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(popcorndrinkitem);
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();

            var popcorndrinkitem = await _context.PopcornDrinkItems.FindAsync(id);
            if (popcorndrinkitem == null) return NotFound();

            return View(popcorndrinkitem);
        }
        [HttpPost]
       
        public async Task<IActionResult> Update(int id, PopcornDrinkItem popcorndrinkitem, IFormFile PictureUrl)
        {
            if (id != popcorndrinkitem.ID) return NotFound();

            if (ModelState.IsValid)
            {
                if (PictureUrl != null && PictureUrl.Length > 0)
                {
                    var filePath = Path.Combine("wwwroot/images", PictureUrl.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await PictureUrl.CopyToAsync(stream);
                    }
                    popcorndrinkitem.PictureUrl = "/images/" + PictureUrl.FileName;
                }
                try
                {
                    _context.Update(popcorndrinkitem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.PopcornDrinkItems.Any(e => e.ID == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var popcorndrinkitem = await _context.PopcornDrinkItems
                .FirstOrDefaultAsync(m => m.ID == id);

            if (popcorndrinkitem == null) return NotFound();

            return View(popcorndrinkitem);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var popcorndrinkitem = await _context.PopcornDrinkItems.FindAsync(id);
            if (popcorndrinkitem != null)
            {
                _context.PopcornDrinkItems.Remove(popcorndrinkitem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Selected()
        {
            // Lấy danh sách PopcornDrinkItem từ cơ sở dữ liệu
            var items = await _context.PopcornDrinkItems.ToListAsync();

            return View("Selected",items);
        }

        [HttpPost]
        public IActionResult Confirm(List<PopcornDrinkItem> items)
        {
            // Xử lý dữ liệu được gửi từ form
            // ...

            return View("Confirmation"); // Tạo View "Confirmation" để hiển thị thông tin đặt hàng
        }
    }
}