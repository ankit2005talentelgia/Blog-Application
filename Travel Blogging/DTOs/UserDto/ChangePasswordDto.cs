using System.ComponentModel.DataAnnotations;

namespace Travel_Blogging.DTOs.UserDto
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage ="Current Password is required")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "minimum length should be 6")]
        [MaxLength(20, ErrorMessage = "maximum length should be 20")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).+$",
        ErrorMessage = "Password must contain uppercase, lowercase, number and special character")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "confirm password is required")]
        [Compare("NewPassword", ErrorMessage = "Confirm password is not same")]
        public string ConfirmPassword { get; set; }
    }
}
