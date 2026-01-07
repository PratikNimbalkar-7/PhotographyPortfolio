using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotographyPortfolio.Models;
using PhotographyPortfolio.Services;
using PhotographyPortfolio.ViewModels;
using System.Net;
using System.Net.Mail;

namespace PhotographyPortfolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly MailService _mailService;

        public HomeController(ApplicationDbContext db,MailService mailService)
        { 
            _db = db;
            _mailService = mailService;
        }


        // ✅ Home page - show 1 photo + 1 video per category
        public async Task<IActionResult> Index(int? categoryId)
        {
            // Load categories (optionally filtered)
            var categoriesQuery = _db.Categories.AsQueryable();

            if (categoryId.HasValue)
                categoriesQuery = categoriesQuery.Where(c => c.Id == categoryId.Value);

            var categories = await categoriesQuery.ToListAsync();

            var models = new List<CategoryMediaViewModel>();

            foreach (var cat in categories)
            {
                // Latest photo
                var photos = await _db.Photos
                    .Where(p => p.CategoryId == cat.Id)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(1)
                    .ToListAsync();

                // Latest video
                var videos = await _db.Videos
                    .Where(v => v.CategoryId == cat.Id)
                    .OrderByDescending(v => v.CreatedAt)
                    .Take(1)
                    .ToListAsync();

                models.Add(new CategoryMediaViewModel
                {
                    Category = cat,
                    Photos = photos,
                    videos = videos
                });
            }

            return View(models);
        }



        // ✅ Display all photos for a specific category
        public IActionResult CategoryPhotos(int id)
        {
            var category = _db.Categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
                return NotFound();

            var photos = _db.Photos
                .Where(p => p.CategoryId == id)
                .OrderByDescending(p => p.Id)
                .ToList();

            ViewBag.CategoryName = category.Name;
            return View(photos);
        }

        // ✅ Details Page for Individual Photo
        public IActionResult Details(int id)
        {
            var photo = _db.Photos
                .FirstOrDefault(p => p.Id == id);

            if (photo == null)
                return NotFound();

            return View(photo);
        }

        // Show all photos in a category
        public IActionResult Category(int id)
        {
            var category = _db.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();

            var photos = _db.Photos.Where(p => p.CategoryId == id).ToList();

            ViewBag.CategoryName = category.Name;
            return View("CategoryPhotos", photos);
        }

        public IActionResult About() => View();
        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(string name, string email, string subject, string message)
        {
            try
            {
                string body = $@"
                    <h3>📩 New Contact Message</h3>
                    <p><strong>Name:</strong> {name}</p>
                    <p><strong>Email:</strong> {email}</p>
                    <p><strong>Subject:</strong> {subject}</p>
                    <p><strong>Message:</strong><br>{message}</p>
                ";

                await _mailService.SendEmailAsync(subject, body, email);

                ViewBag.Message = "✅ Thank you for contacting me! I’ll reply shortly.";
                ViewBag.AlertType = "success";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "❌ Oops! Something went wrong while sending your message.";
                ViewBag.AlertType = "danger";
                ViewBag.Error = ex.Message;
            }

            return View();
        }
    }
}

