using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewModels.Service
{
    public class ServiceListVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public bool IsActive { get; set; }
    }
}
