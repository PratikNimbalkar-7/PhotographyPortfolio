using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotographyPortfolio.Models;
using PhotographyPortfolio.Services;
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


        // ✅ Home page - show only 1 photo per category
        public async Task<IActionResult> Index()
        {
            var categories = await _db.Categories.ToListAsync();

            var photos = await _db.Photos
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            ViewBag.Categories = categories;
          
            return View(photos);
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

