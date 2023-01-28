using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewModels.ProductCategory
{
    public class ProductCategoryCreateVM
    {
        [Required]
        public string Name { get; set; }
    }
}
