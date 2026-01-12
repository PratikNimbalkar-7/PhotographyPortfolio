namespace PhotographyPortfolio.Models.ViewModels
{
    public class GalleryGroupVM
    {
        public Category Category { get; set; }
        public List<Photo> Photos { get; set; } = new();
        public List<Video> Videos { get; set; } = new();
    }
}
