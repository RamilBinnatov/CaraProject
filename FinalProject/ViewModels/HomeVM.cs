using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewModels
{
    public class HomeVM
    {   
        public IEnumerable<Models.Banner> Banners { get; set; }
        public IEnumerable<Models.Social> Socials { get; set; }
        public IEnumerable<Models.Product> BestSelling { get; set; }
        public IEnumerable<Models.Product> NewArrivals { get; set; }
        public IEnumerable<Models.Poster> Posters { get; set; }
        public IEnumerable<Models.Board> Boards { get; set; }
        public IEnumerable<Models.Hero> Heroes { get; set; }

    }
}
