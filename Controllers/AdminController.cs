using Microsoft.AspNetCore.Mvc;
using PhotographyPortfolio.Models;
using Microsoft.EntityFrameworkCore;

namespace PhotographyPortfolio.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private const string SessionKey = "IsAdmin";
        private const string AdminUser = "admin";
        private const string AdminPass = "admin@123"; // change after first run

        public AdminController(ApplicationDbContext db) { _db = db; }

        private bool IsLoggedIn => HttpContext.Session.GetString(SessionKey) == "1";

        public IActionResult Login()
        {
            if (IsLoggedIn)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (username == AdminUser && password == AdminPass)
            {
                HttpContext.Session.SetString(SessionKey, "1");
                TempData["LoginSuccess"] = "Welcome back, Admin!";
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Invalid credentials.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove(SessionKey);
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn) return RedirectToAction("Login");
            var photos = await _db.Photos.Include(p => p.Category).OrderByDescending(p => p.CreatedAt).ToListAsync();
            return View(photos);
        }

        public async Task<IActionResult> Create()
        {
            if (!IsLoggedIn) return RedirectToAction("Login");
            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Photo model, List<IFormFile> images)
        {
            if (!IsLoggedIn) return RedirectToAction("Login");

            // Reload dropdowns if you re-render the view
            if (!ModelState.IsValid || images == null || images.Count == 0)
            {
                ViewBag.Categories = await _db.Categories.ToListAsync();
                if (images == null || images.Count == 0)
                    ModelState.AddModelError("images", "Please select at least one image.");
                return View(model);
            }

            var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsRoot))
                Directory.CreateDirectory(uploadsRoot);

            // Optional: basic validation
            long maxBytes = 5 * 1024 * 1024; // 5 MB
            string[] allowed = [".jpg", ".jpeg", ".png", ".webp"];

            int added = 0;
            foreach (var file in images)
            {
                if (file == null || file.Length == 0) continue;

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowed.Contains(ext))
                {
                    // skip invalid file types
                    continue;
                }
                if (file.Length > maxBytes)
                {
                    // skip too-large files
                    continue;
                }

                var newName = $"{Guid.NewGuid()}{ext}";
                var savePath = Path.Combine(uploadsRoot, newName);

                using (var fs = new FileStream(savePath, FileMode.Create))
                    await file.CopyToAsync(fs);

                // Create a new Photo per file
                var photo = new Photo
                {
                    Title = string.IsNullOrWhiteSpace(model.Title)
                                ? Path.GetFileNameWithoutExtension(file.FileName)
                                : model.Title,
                    Description = model.Description,
                    CategoryId = model.CategoryId,
                    ImagePath = "/uploads/" + newName,
                    // If your Photo has UploadDate or other props, set them here
                    // UploadDate = DateTime.Now
                };

                _db.Photos.Add(photo);
                added++;
            }

            if (added == 0)
            {
                ViewBag.Categories = await _db.Categories.ToListAsync();
                ModelState.AddModelError("", "No valid images were uploaded.");
                return View(model);
            }

            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = $"{added} photo(s) uploaded successfully!";
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Edit(int id)
        {
            if (!IsLoggedIn) return RedirectToAction("Login");
            var photo = await _db.Photos.FindAsync(id);
            if (photo == null) return NotFound();
            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View(photo);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Photo model, IFormFile? image)
        {
            if (!IsLoggedIn) return RedirectToAction("Login");
            var photo = await _db.Photos.FindAsync(id);
            if (photo == null) return NotFound();

            photo.Title = model.Title;
            photo.Description = model.Description;
            photo.CategoryId = model.CategoryId;

            if (image != null && image.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(uploads, fileName);
                using var fs = new FileStream(filePath, FileMode.Create);
                await image.CopyToAsync(fs);
                photo.ImagePath = "/uploads/" + fileName;
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsLoggedIn) return RedirectToAction("Login");
            var photo = await _db.Photos.FindAsync(id);
            if (photo == null) return NotFound();
            _db.Photos.Remove(photo);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Category management (simple)
        public async Task<IActionResult> Categories()
        {
            if (!IsLoggedIn) return RedirectToAction("Login");
            var cats = await _db.Categories.ToListAsync();
            return View(cats);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(string name)
        {
            if (!IsLoggedIn) return RedirectToAction("Login");
            if (!string.IsNullOrWhiteSpace(name))
            {
                _db.Categories.Add(new Category { Name = name.Trim() });
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Categories");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (!IsLoggedIn) return RedirectToAction("Login");
            var c = await _db.Categories.FindAsync(id);
            if (c != null)
            {
                _db.Categories.Remove(c);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Categories");
        }
    }
}
