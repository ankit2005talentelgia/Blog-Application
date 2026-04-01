using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Travel_Blogging.DTOs.PostDto
{
    public class CreatePostDto: IValidatableObject
    {
        public int? Id { get;set; }

        [Required(ErrorMessage ="Title is required")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Title must contain only characters")]
        [MaxLength(100,ErrorMessage ="Title is less than 100 length")]
        [MinLength(10,ErrorMessage ="Title is more than 10 length")]
        public string? Title {  get; set; }

        [Required(ErrorMessage ="Description is required")]
        [MaxLength(500, ErrorMessage = "Description is less than 500 length")]
        [MinLength(100, ErrorMessage = "Description is more than 100 length")]
        [RegularExpression(@"^(?!.*\d{6,}).*$",
            ErrorMessage = "Description cannot contain more than 5 consecutive numbers")]
        public string? Description { get; set; }

        [Required(ErrorMessage ="Location is required")]
        [MaxLength(100, ErrorMessage = "Location is less than 100 length")]
        [MinLength(10, ErrorMessage = "Location is more than 10 length")]
        [RegularExpression(@"^(?!.*\d{5,})[A-Za-z0-9\s,]+$",
            ErrorMessage = "Location can contain maximum 4 numbers and rest should be characters")]
        public string? Location { get; set; }

        //[Required(ErrorMessage = "Image is required")]
        public IFormFile? Image { get; set; }

        public string? ExistingImageUrl { get; set; }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Image == null && string.IsNullOrEmpty(ExistingImageUrl))
            {
                yield return new ValidationResult(
                    "Image is required",
                    new[] { nameof(Image) }
                );
            }
        }
    }

}
