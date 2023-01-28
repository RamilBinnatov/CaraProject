using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewModels.Product
{
    public class ProductListVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Price { get; set; }
        public int Stock_count { get; set; }
        public int BrandId { get; set; }
        public string Brand { get; set; }
        public string MainImage { get; set; }
    }
}
