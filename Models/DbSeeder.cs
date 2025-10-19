using System.IO;

namespace PhotographyPortfolio.Models
{
    public static class DbSeeder
    {
        public static void EnsureSeedData(ApplicationDbContext db, IWebHostEnvironment env)
        {
            if (db.Categories.Any() || db.Photos.Any()) return;

            var cats = new[]
            {
                new Category { Name = "Nature" },
                new Category { Name = "Wedding" },
                new Category { Name = "Street" }
            };
            db.Categories.AddRange(cats);
            db.SaveChanges();

            // create placeholder images
            var uploads = Path.Combine(env.ContentRootPath, "wwwroot", "uploads");
            Directory.CreateDirectory(uploads);

            for (int i = 1; i <= 6; i++)
            {
                var fname = $"demo{i}.png";
                var fpath = Path.Combine(uploads, fname);
                if (!File.Exists(fpath))
                {
                    // create a small placeholder PNG (1x1 transparent) - simple binary PNG
                    File.WriteAllBytes(fpath, Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGNgYAAAAAMAASsJTYQAAAAASUVORK5CYII="));
                }

                db.Photos.Add(new Photo
                {
                    Title = $"Demo Photo {i}",
                    Description = "Sample demo photo.",
                    ImagePath = "/uploads/" + fname,
                    CategoryId = cats[i % cats.Length].Id
                });
            }

            db.SaveChanges();
        }
    }
}
