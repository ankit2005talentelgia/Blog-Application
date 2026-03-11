using System.ComponentModel.DataAnnotations;

namespace Travel_Blogging.DTOs.PostDto
{
    public class CreateCommentDto
    {
        [Required(ErrorMessage = "Comment is required")]
        [MinLength(5, ErrorMessage = "Comment must be at least 5 characters")]
        [MaxLength(500, ErrorMessage = "Comment cannot exceed 500 characters")]
        public string Content { get; set; }

        [Required(ErrorMessage = "PostId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid PostId")]
        public int PostId { get; set; }
    }
}
