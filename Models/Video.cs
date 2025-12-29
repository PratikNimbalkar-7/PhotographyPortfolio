namespace PhotographyPortfolio.Models
{
    public class video
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string VideoPath { get; set; }   // /videos/wedding/abc.mp4

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
