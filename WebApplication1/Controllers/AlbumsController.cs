using Gallery.Data;
using Gallery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Gallery.Controllers
{
        [Authorize]
        public class AlbumsController : Controller
        {
            private readonly ApplicationDbContext _context;

            public AlbumsController(ApplicationDbContext context)
            {
                _context = context;
            }

        // List Albums
        [AllowAnonymous]
        public async Task<IActionResult> Index(int page = 1)
            {
                int pageSize = 5;
                var albums = await _context.Albums
                    .Include(a => a.Photos)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                return View(albums);
            }

            // Create Album
            [HttpPost]
            [AllowAnonymous]
            public async Task<IActionResult> Create(Album album)
            {
                if (ModelState.IsValid)
                {
                    album.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    _context.Albums.Add(album);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(album);
            }

            // Delete Album
            [HttpPost]
            [Authorize(Policy = "RequireAdminRole")]
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Delete(int id)
            {
                var album = await _context.Albums.FindAsync(id);
                if (album == null || album.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    return NotFound();
                }
                _context.Albums.Remove(album);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }


    
}