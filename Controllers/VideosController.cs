using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotographyPortfolio.Models;
using PhotographyPortfolio.ViewModels;

namespace PhotographyPortfolio.Controllers
{
    public class VideosController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public VideosController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // GET: /Videos
        public async Task<IActionResult> Index(int? categoryId)
        {
            var videos = _db.Videos.Include(v => v.Category).AsQueryable();
            if (categoryId.HasValue) videos = videos.Where(v => v.CategoryId == categoryId.Value);

            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View(await videos.OrderByDescending(v => v.CreatedAt).ToListAsync());
        }

        // GET: /Videos/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var video = await _db.Videos.Include(v => v.Category).FirstOrDefaultAsync(v => v.Id == id);
            if (video == null) return NotFound();
            return View(video);
        }

        // GET: /Videos/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = _db.Categories.ToList();
            var vm = new VideoViewModel
            {
                CreatedAt = DateTime.Now // optional default
            };
            return View(vm);
        }

        // POST: /Videos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VideoViewModel vm)
        {
            ViewBag.Categories = _db.Categories.ToList();

            // show modelstate errors for debugging if you want:
            if (!ModelState.IsValid)
            {
                // return view with validation messages
                return View(vm);
            }

            var file = vm.File!;
            // extra server-side validation
            var allowed = new[] { ".mp4", ".webm", ".ogg" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            const long maxBytes = 700L * 1024 * 1024; // 200MB example

            if (!allowed.Contains(ext))
            {
                ModelState.AddModelError(nameof(vm.File), "Only MP4/WebM/OGG allowed.");
                return View(vm);
            }

            if (file.Length == 0 || file.Length > maxBytes)
            {
                ModelState.AddModelError(nameof(vm.File), "File is empty or too large.");
                return View(vm);
            }

            var uploads = Path.Combine(_env.WebRootPath, "uploads", "videos");
            Directory.CreateDirectory(uploads);

            var uniqueFile = Guid.NewGuid().ToString("N") + ext;
            var fullPath = Path.Combine(uploads, uniqueFile);

            using (var fs = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }

            // map to entity
            var video = new Video
            {
                Title = vm.Title,
                Description = vm.Description,
                CategoryId = vm.CategoryId,
                MediaPath = "/uploads/videos/" + uniqueFile,
                CreatedAt = vm.CreatedAt ?? DateTime.UtcNow
            };

            _db.Videos.Add(video);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Video added!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Videos/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var video = await _db.Videos.FindAsync(id);
            if (video == null) return NotFound();
            ViewBag.Categories = _db.Categories.ToList();
            return View(video);
        }

        // POST: /Videos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Video model, IFormFile? file)
        {
            var existing = await _db.Videos.FindAsync(id);
            if (existing == null) return NotFound();

            ViewBag.Categories = _db.Categories.ToList();
            if (!ModelState.IsValid) return View(model);

            // Replace file if new uploaded
            if (file != null && file.Length > 0)
            {
                var allowed = new[] { ".mp4", ".webm", ".ogg" };
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError("MediaPath", "Only MP4/WebM/OGG video files are allowed.");
                    return View(model);
                }

                var uploads = Path.Combine(_env.WebRootPath, "uploads", "videos");
                Directory.CreateDirectory(uploads);

                var unique = Guid.NewGuid().ToString() + ext;
                var fullPath = Path.Combine(uploads, unique);

                using (var fs = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(fs);
                }

                // delete old file if exists
                if (!string.IsNullOrEmpty(existing.MediaPath))
                {
                    var old = Path.Combine(_env.WebRootPath, existing.MediaPath.TrimStart('/'));
                    if (System.IO.File.Exists(old)) System.IO.File.Delete(old);
                }

                existing.MediaPath = "/uploads/videos/" + unique;
            }

            existing.Title = model.Title;
            existing.Description = model.Description;
            existing.CategoryId = model.CategoryId;
            // existing.MediaType remains "Video"
            await _db.SaveChangesAsync();

            TempData["Success"] = "Video updated.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Videos/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var video = await _db.Videos.Include(v => v.Category).FirstOrDefaultAsync(v => v.Id == id);
            if (video == null) return NotFound();
            return View(video);
        }

        // POST: /Videos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var video = await _db.Videos.FindAsync(id);
            if (video == null) return NotFound();

            // delete file
            if (!string.IsNullOrEmpty(video.MediaPath))
            {
                var full = Path.Combine(_env.WebRootPath, video.MediaPath.TrimStart('/'));
                if (System.IO.File.Exists(full)) System.IO.File.Delete(full);
            }

            _db.Videos.Remove(video);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Video deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
