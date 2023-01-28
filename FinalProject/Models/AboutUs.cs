using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class AboutUs : BaseEntity
    {
        public string Image { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string TextSlider { get; set; }
    }
}
