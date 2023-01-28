using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class PageHeader : BaseEntity
    {
        public string Title { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Page { get; set; }
        [NotMapped]
        [Required]
        public IFormFile Photo { get; set; }
    }
}
