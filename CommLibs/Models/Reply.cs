using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CommLibs.Models
{
    public class Reply
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public int ProductId { get; set; }
        [JsonIgnore]
        public Product? Product { get; set; }
        public int UserId { get; set; }
        [JsonIgnore]
        public User? User { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; } = DateTime.Now;
    }
}
