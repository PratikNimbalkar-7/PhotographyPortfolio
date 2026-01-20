using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PhotographyPortfolio.Models;
using PhotographyPortfolio.Models.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


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
        public async Task<IActionResult> Index()
        {
            var videos = await _db.Videos.ToListAsync();
            return View(videos);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View();
        }


        [HttpPost]

        [RequestSizeLimit(52428800)] // 50 MB
        [RequestFormLimits(MultipartBodyLengthLimit = 52428800)]
        public async Task<IActionResult> Create([FromForm] VideoViewModel vm)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid data" });

            if (vm.VideoFile == null || vm.VideoFile.Length == 0)
                return Json(new { success = false, message = "Please select a video file" });

            // 2️ File size validation (50 MB)
            const long MaxVideoSize = 50 * 1024 * 1024; // 50 MB
            if (vm.VideoFile.Length > MaxVideoSize)
                return Json(new { success = false, message = "Video size must be less than 50 MB" });

            if (vm.CategoryId <= 0)
                return Json(new { success = false, message = "Please select a category" });

            var category = await _db.Categories.FindAsync(vm.CategoryId);
            if (category == null)
                return Json(new { success = false, message = "Category not found" });

            string folder = Path.Combine(_env.WebRootPath, "videos", category.Name);
            Directory.CreateDirectory(folder);

            string fileName = Guid.NewGuid() + Path.GetExtension(vm.VideoFile.FileName);
            string filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await vm.VideoFile.CopyToAsync(stream);
            }

            var video = new Video
            {
                Title = vm.Title,
                Description = vm.Description ?? "",
                CategoryId = vm.CategoryId,
                VideoPath = $"videos/{category.Name}/{fileName}",
                CreatedAt = DateTime.Now
            };

            _db.Videos.Add(video);
            await _db.SaveChangesAsync();

            //return Json(Index ,new
            //{
            //    success = true,
            //    message = "  Video uploaded successfully "
            //});

            return Json(new
            {
                success = true,
                message = "Video uploaded successfully",
                redirectUrl = Url.Action("Index", "videos")
            });


        }










    }
}






