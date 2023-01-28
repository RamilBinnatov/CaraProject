using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewModels.Blog
{
    public class BlogDetailVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Creator { get; set; }
        public string CategoryName { get; set; }
        public DateTime CreateDate { get; set; }
        public int TagId { get; set; }
        public List<Models.Tag> Tags { get; set; }
        public IEnumerable<int> Blog_tagList { get; set; }
        public ICollection<BlogImage> BlogImages { get; set; }


    }
}
