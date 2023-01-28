using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class Size : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<Product_Size> Product_Sizes { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }

    }
}
