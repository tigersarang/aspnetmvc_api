using System.ComponentModel.DataAnnotations;

namespace CommLibs.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
