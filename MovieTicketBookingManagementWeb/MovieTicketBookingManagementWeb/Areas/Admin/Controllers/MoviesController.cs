﻿using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingManagementWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace MovieTicketBookingManagementWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class MoviesController : Controller
    {
        
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách phim
        public IActionResult Index(string? searchQuery)
        {
            var movies = _context.Movies.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                movies = movies.Where(m => m.Title.Contains(searchQuery));
            }

            ViewData["SearchQuery"] = searchQuery;
            return View(movies.ToList()); // Đảm bảo trả về danh sách phim
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(Movie movie, IFormFile
        PosterUrl)
        {
            if (ModelState.IsValid)
            {
                if (movie.ReleaseDate != null)
                    {
                    var releaseDate = movie.ReleaseDate;
                    movie.ReleaseDate = releaseDate;
                }
                if (PosterUrl != null && PosterUrl.Length > 0)
                {
                    var filePath = Path.Combine("wwwroot/images", PosterUrl.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await PosterUrl.CopyToAsync(stream);
                    }
                    movie.PosterUrl = "/images/" + PosterUrl.FileName;
                }
                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(movie);
        }
        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();

            return View(movie);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, Movie movie, IFormFile PosterUrl)
        {
            if (id != movie.ID) return NotFound();

            if (ModelState.IsValid)
            {
                if (PosterUrl != null && PosterUrl.Length > 0)
                {
                    var filePath = Path.Combine("wwwroot/images", PosterUrl.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await PosterUrl.CopyToAsync(stream);
                    }
                    movie.PosterUrl = "/images/" + PosterUrl.FileName;
                }
                _context.Update(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }
        // Xem chi tiết phim
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.ID == id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();

            return View(movie);
        }

        // Xử lý xóa phim
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Trailer(int id)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.ID == id);
            if (movie == null || string.IsNullOrEmpty(movie.TrailerID))
            {
                return NotFound();
            }
            return View(movie);
        }

    }
}
