using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace Travel_Blogging.Models
{
    [Table("Users")]
    public class UserModel:BaseEntity
    {
        public int Id { get; set; }
        public string?  Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string? EmailConfirmationToken { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<PostModel>? Posts { get; set; }
        public List<ReviewModel>? Comments { get; set; }
    }
}

