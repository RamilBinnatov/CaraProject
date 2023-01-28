using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewModels
{
    public class BlogVM
    {
        public IEnumerable<Models.Blog> Blogs { get; set; }
        public IEnumerable<PageHeader> PageHeaders { get; set; }
    }
}
