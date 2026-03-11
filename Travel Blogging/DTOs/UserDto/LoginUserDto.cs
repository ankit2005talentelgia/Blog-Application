using System.ComponentModel.DataAnnotations;

namespace Travel_Blogging.DTOs.UserDto
{
    public class LoginUserDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        [MaxLength(50, ErrorMessage = "Email too long")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
