using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotographyPortfolio.Models;
using System.Net;
using System.Net.Mail;

namespace PhotographyPortfolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db) { _db = db; }

        public async Task<IActionResult> Index(int? categoryId)
        {
            var photos = _db.Photos.Include(p => p.Category).AsQueryable();
            if (categoryId.HasValue) photos = photos.Where(p => p.CategoryId == categoryId.Value);
            var categories = await _db.Categories.ToListAsync();

            ViewBag.Categories = categories;
            return View(await photos.OrderByDescending(p => p.CreatedAt).ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var photo = await _db.Photos.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (photo == null) return NotFound();
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
        public IActionResult Contact(string name, string email, string subject, string message)
        {
            try
            {
                // ? Build email body
                string body = $@"
                    <h3>New Contact Message</h3>
                    <p><strong>Name:</strong> {name}</p>
                    <p><strong>Email:</strong> {email}</p>
                    <p><strong>Subject:</strong> {subject}</p>
                    <p><strong>Message:</strong><br>{message}</p>
                ";

                // ? Configure SMTP client (example: Gmail)
                using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("yourgmail@gmail.com", "your-app-password");
                    smtp.EnableSsl = true;

                    var mail = new MailMessage();
                    mail.From = new MailAddress("yourgmail@gmail.com", "Photography Portfolio");
                    mail.To.Add("yourgmail@gmail.com"); // You can add multiple recipients
                    mail.Subject = $"New Contact Form Message: {subject}";
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    smtp.Send(mail);
                }

                ViewBag.Message = "? Thank you for contacting me! I’ll reply shortly.";
                ViewBag.AlertType = "success";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "? Oops! Something went wrong while sending your message.";
                ViewBag.AlertType = "danger";
                ViewBag.Error = ex.Message;
            }

            return View();
        }
    }
}

