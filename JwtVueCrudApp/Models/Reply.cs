using System.ComponentModel.DataAnnotations;

namespace JwtVueCrudApp.Models
{
    public class Reply
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}
