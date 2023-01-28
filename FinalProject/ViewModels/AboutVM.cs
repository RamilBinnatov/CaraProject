using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewModels
{
    public class AboutVM
    {
        public IEnumerable<PageHeader> PageHeaders { get; set; }
        public AboutUs AboutUs { get; set; }

    }
}
