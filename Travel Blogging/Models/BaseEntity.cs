namespace Travel_Blogging.Models
{
    public class BaseEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }
}
