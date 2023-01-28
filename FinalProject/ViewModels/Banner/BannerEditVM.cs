using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewModels.Banner
{
    public class BannerEditVM
    {
        public string Header { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public IFormFile Photo { get; set; }
    }
}
