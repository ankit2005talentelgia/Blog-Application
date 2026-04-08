using System.ComponentModel.DataAnnotations;

namespace Travel_Blogging.DTOs.ContactDto
{
    public class ContactDto
    {
        [Required(ErrorMessage ="name is required")]
        [MinLength(3, ErrorMessage = "minimum length should be 3")]
        [MaxLength(50, ErrorMessage = "maximum length should be 50")]
        [RegularExpression(@"^[A-Za-z]+(?: [A-Za-z]+)*$",
            ErrorMessage = "Only letters allowed and only one space between words")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        [MaxLength(50, ErrorMessage = "Email too long")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "subject is required")]
        [MinLength(10, ErrorMessage = "minimum length should be 10")]
        [MaxLength(100, ErrorMessage = "maximum length should be 100")]
        [RegularExpression(@"^[A-Za-z]+(?: [A-Za-z]+)*$",
            ErrorMessage = "Only letters allowed")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "message is required")]
        [MinLength(50, ErrorMessage = "minimum length should be 50")]
        [MaxLength(1000, ErrorMessage = "maximum length should be 1000")]
        public string Message { get; set; }
    }
}
