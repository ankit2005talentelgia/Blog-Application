using System.ComponentModel.DataAnnotations;

namespace Travel_Blogging.DTOs.UserDto
{
    public class UpdateProfileDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MinLength(3, ErrorMessage = "minimum length should be 3")]
        [MaxLength(50, ErrorMessage = "maximum length should be 50")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name should contain only letters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        [MaxLength(50, ErrorMessage = "Email too long")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
    }
}
