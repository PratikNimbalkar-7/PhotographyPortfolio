using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PhotographyPortfolio.Models.ViewModels
{
    public class VideoViewModel
    {
        //public int Id { get; set; }

        //[Required(ErrorMessage = "Title is required")]
        //[StringLength(150)]
        //public string? Title { get; set; }

        //[StringLength(500)]
        //public string? Description { get; set; }

        //[Required(ErrorMessage = "Category is required")]
        //public int CategoryId { get; set; }

        //// Used only for upload
        //[Display(Name = "Upload Video")]
        ////public IFormFile VideoFile { get; set; }

        //// Used for edit & display
        //public string VideoPath { get; set; }


        ////public IFormFile videoFile { get; set; }

        //public IFormFile videoFile { get; set; }
        ////public List<IFormFile>? VideoFile { get; set; }
        ///



        [Required]
        public string? Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Please select a video")]
        public IFormFile VideoFile { get; set; }
    }
}
