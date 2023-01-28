using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class BlogImage : BaseEntity
    {
        public string Image { get; set; }
        public bool IsMain { get; set; } = false;
        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
