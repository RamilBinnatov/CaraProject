using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class ProductCategory : BaseEntity
    {
        [Required(ErrorMessage = "Name can't be empty")]
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
