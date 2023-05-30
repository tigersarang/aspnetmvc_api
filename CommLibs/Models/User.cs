using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CommLibs.Models
{
    public class User
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        public int? ProductId { get; set; }
        [JsonIgnore]

        public List<Product>? Products { get; set; }

        public int? RoleId { get; set; }

        public ICollection<Role> Roles { get; set; }
    }
}
