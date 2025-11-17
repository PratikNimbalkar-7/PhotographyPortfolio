using PhotographyPortfolio.Models;

namespace PhotographyPortfolio.ViewModels
{
    public class CategoryMediaViewModel
    {
        public Category Category { get; set; } = null!;
        public IEnumerable<Photo> Photos { get; set; } = Enumerable.Empty<Photo>();
        public IEnumerable<Video> Videos { get; set; } = Enumerable.Empty<Video>();
    }
}
