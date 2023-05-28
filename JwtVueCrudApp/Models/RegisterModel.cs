using System.ComponentModel.DataAnnotations;

namespace JwtVueCrudApp.Models
{
    public class RegisterModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        public List<string> Roles { get; set; }
    }
}
