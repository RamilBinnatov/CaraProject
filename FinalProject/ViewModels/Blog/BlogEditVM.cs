using FinalProject.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewModels.Blog
{
    public class BlogEditVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int CategoryId { get; set; }
        public ICollection<BlogImage> Images { get; set; }
        public List<IFormFile> Photos { get; set; }
    }
}
