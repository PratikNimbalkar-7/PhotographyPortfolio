using System.ComponentModel.DataAnnotations;

namespace PhotographyPortfolio.ViewModels
{
    public class VideoViewModel
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        // optional date chosen by user
        public DateTime? CreatedAt { get; set; }

        [Required(ErrorMessage = "Please select a video file.")]
        public IFormFile? File { get; set; }
    }
}
