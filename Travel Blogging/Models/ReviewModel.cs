using System.ComponentModel.DataAnnotations.Schema;

namespace Travel_Blogging.Models
{
    [Table("Comments")]
    public class ReviewModel:BaseEntity
    {
        public int Id { get; set; }
        public string? Content {  get; set; }
        public int PostId { get; set; }
        public PostModel? Post {  get; set; }
        public int UserId {  get; set; }
        public UserModel? User {  get; set; }
    }
}
