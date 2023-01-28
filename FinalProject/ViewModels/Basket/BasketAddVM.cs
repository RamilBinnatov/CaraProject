using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewModels.Basket
{
    public class BasketAddVM
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int BasketId { get; set; }
        public Models.Basket Basket { get; set; }
        public int ProductId { get; set; }
        public Models.Product Product { get; set; }
    }
}
