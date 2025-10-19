using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotographyPortfolio.Models
{
    public class Photo
    {
        public int Id { get; set; }

        [Required] public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ImagePath { get; set; } = string.Empty;

        // foreign key
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
