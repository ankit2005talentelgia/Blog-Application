using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace Travel_Blogging.DTOs.PostDto
{
    public class CreatePostDto
    {
        [Required(ErrorMessage ="Title is required")]
        [MaxLength(100,ErrorMessage ="Title is less than 100 length")]
        [MinLength(10,ErrorMessage ="Title is more than 10 length")]
        public string Title {  get; set; }

        [Required(ErrorMessage ="Description is required")]
        [MaxLength(500, ErrorMessage = "Description is less than 500 length")]
        [MinLength(100, ErrorMessage = "Description is more than 100 length")]
        public string Description { get; set; }

        [Required(ErrorMessage ="Location is required")]
        [StringLength(100,ErrorMessage ="Location is less than 100 length")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Image is required")]
        public IFormFile Image { get; set; }
    }
}


//if (Image != null)
//{
//    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
//    var extension = Path.GetExtension(Image.FileName).ToLower();

//    if (!allowedExtensions.Contains(extension))
//    {
//        return BadRequest("Only JPG, JPEG, PNG images allowed");
//    }

//    if (Image.Length > 2 * 1024 * 1024)
//    {
//        return BadRequest("Image size must be less than 2MB");
//    }
//}
