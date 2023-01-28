using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewModels.Service
{
    public class ServiceCreateVM
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public IFormFile Photo { get; set; }
    }
}
