using System.ComponentModel.DataAnnotations;

namespace Travel_Blogging.DTOs.UserDto
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage ="Name is required")]
        [MinLength(3, ErrorMessage = "minimum length should be 3")]
        [MaxLength(50, ErrorMessage = "maximum length should be 50")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name should contain only letters")]
        public string Name {  get; set; }

        [Required(ErrorMessage ="Email is required")]
        [EmailAddress(ErrorMessage ="Invalid email")]
        [MaxLength(50, ErrorMessage = "Email too long")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Password is required")]
        [MinLength(6,ErrorMessage ="minimum length should be 6")]
        [MaxLength(20,ErrorMessage ="maximum length should be 20")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).+$",
        ErrorMessage = "Password must contain uppercase, lowercase, number and special character")]
        public string Password { get; set; }

        [Required(ErrorMessage ="confirm password is required")]
        [Compare("Password",ErrorMessage ="Confirm password is not same")]
        public string ConfirmPassword {  get; set; }
    }
}
