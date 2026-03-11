using System.ComponentModel.DataAnnotations.Schema;

namespace Travel_Blogging.Models
{
    [Table("Posts")]
    public class PostModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string ImageUrl { get; set; }
        public string AuthorId { get; set; }
        public UserModel? User { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<CommentModel>? Comments { get; set; }
    }
}
