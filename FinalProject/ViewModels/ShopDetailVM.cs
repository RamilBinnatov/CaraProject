using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewModels
{
    public class ShopDetailVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string MainImage { get; set; }
        public string CategoryName { get; set; }
        public string ProductSize { get; set; }
        public IEnumerable<ProductImage> ProductImages { get; set; }
        public IEnumerable<Models.Product> FeaturedProducts { get; set; }
        public int SizeId { get; set; }
        public List<Models.Size> Sizes { get; set; }
        public IEnumerable<int> Product_sizeList { get; set; }
    }
}
