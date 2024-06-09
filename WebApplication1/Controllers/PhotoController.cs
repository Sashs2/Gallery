using Gallery.Data;
using Gallery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Gallery.Controllers
{
    [Authorize]
    public class PhotoController:Controller
    {
        private readonly ApplicationDbContext _context;

        public PhotoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // List Photos
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 5;
            var photos = await _context.Photos
                .Include(a => a.Likes)
                .Include(p => p.DisLikes)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return View(photos);
        }

      

    }
}
