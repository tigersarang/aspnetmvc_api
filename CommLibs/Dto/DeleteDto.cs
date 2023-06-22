using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommLibs.Dto
{
    public class DeleteDto
    {
        public int? Id { get; set; }
        public string? LinkFileName { get; set; }
        public bool? IsDbDelete { get; set; }
    }
}
