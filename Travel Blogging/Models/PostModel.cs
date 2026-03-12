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
        public int AuthorId { get; set; }
        public UserModel? Author { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<CommentModel>? Comments { get; set; }
    }
}
