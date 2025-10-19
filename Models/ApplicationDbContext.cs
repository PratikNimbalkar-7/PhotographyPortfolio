using Microsoft.EntityFrameworkCore;

namespace PhotographyPortfolio.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

        public DbSet<Photo> Photos { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
