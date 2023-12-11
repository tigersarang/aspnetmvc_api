using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CommLibs.Models
{
    public class ProductFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string LInkFileName { get; set; }
        public int? ProductId { get; set; }
        [JsonIgnore]
        public Product? Product { get; set; }
    }
}
