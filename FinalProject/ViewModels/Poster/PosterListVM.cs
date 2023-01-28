using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewModels.Poster
{
    public class PosterListVM
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public string Title { get; set; }
        public string Header { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}
