using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewModels.Size
{
    public class SizeCreateVM
    {
        [Required]
        public string Name { get; set; }
    }
}
