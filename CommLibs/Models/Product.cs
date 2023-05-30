using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommLibs.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Content { get; set; }

        public List<Reply>? Replies { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? UpdatedDate { get; set; } = DateTime.Now;

    }
}
