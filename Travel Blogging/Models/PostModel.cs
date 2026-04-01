using System.ComponentModel.DataAnnotations.Schema;
using Travel_Blogging.Models.Enums;

namespace Travel_Blogging.Models
{
    [Table("Posts")]
    public class PostModel:BaseEntity
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? ImageUrl { get; set; }
        public int AuthorId { get; set; }
        public UserModel? Author { get; set; }

        public PostStatus Status { get; set; } = PostStatus.Draft;
        public List<ReviewModel>? Comments { get; set; }
    }
}
