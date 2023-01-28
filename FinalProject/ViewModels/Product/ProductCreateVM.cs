using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewModels.Product
{
    public class ProductCreateVM
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Price { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Stock_count { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        [Required]
        public List<IFormFile> Photos { get; set; }
        public int SizeId { get; set; }
        public List<Models.Size> Size { get; set; }
        public IEnumerable<int> Product_sizeList { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
