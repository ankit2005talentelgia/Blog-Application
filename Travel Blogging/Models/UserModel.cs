using System.ComponentModel.DataAnnotations.Schema;

namespace Travel_Blogging.Models
{
    [Table("Users")]
    public class UserModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<PostModel>? Posts { get; set; }
        public List<CommentModel>? Comments { get; set; }
    }
}
