using System.ComponentModel.DataAnnotations;

namespace Travel_Blogging.DTOs.PostDto
{
    public class EditPostDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Location { get; set; }

        public IFormFile? Image { get; set; }


    }
}
