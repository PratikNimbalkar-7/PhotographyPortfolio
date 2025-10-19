using System.ComponentModel.DataAnnotations;

namespace PhotographyPortfolio.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; } = string.Empty;
        public ICollection<Photo>? Photos { get; set; }
    }
}
