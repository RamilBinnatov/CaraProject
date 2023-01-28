using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class Product_Size : BaseEntity
    {
        public int SizeId { get; set; }
        public int ProductId { get; set; }

        public Product Product { get; set; }
        public Size Size { get; set; }

    }
}
