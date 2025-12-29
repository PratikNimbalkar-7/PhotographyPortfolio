using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhotographyPortfolio.Models;

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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(VideoViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _db.Categories
                                          .OrderBy(c => c.Name)
                                          .ToListAsync();

                ViewData["Categories"] = categories
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToList();

                return View(vm);
            }

            string videoPath = null;

            if (vm.VideoFile != null)
            {
                string folder = Path.Combine(_env.WebRootPath, "videos");
                Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid() + Path.GetExtension(vm.VideoFile.FileName);
                string fullPath = Path.Combine(folder, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await vm.VideoFile.CopyToAsync(stream);

                videoPath = "/videos/" + fileName;
            }

            var video = new video
            {
                Title = vm.Title,
                Description = vm.Description,
                VideoPath = videoPath,
                CategoryId = vm.CategoryId,
                CreatedAt = DateTime.Now
            };

            _db.Videos.Add(video);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
