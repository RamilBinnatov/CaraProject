using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewModels
{
    public class BlogDetailsVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Creator { get; set; }
        public string MainImage { get; set; }

        public List<Models.Tag> Tags { get; set; }
        public IEnumerable<BlogImage> blogImages { get; set; }
        public DateTime Createdate { get; set; }
        public IEnumerable<Models.Blog> RecentPosts { get; set; }
        public IEnumerable<Models.BlogCategory> Categories { get; set; }
        public IEnumerable<Models.Social> Socials { get; set; }
        public IEnumerable<Models.Tag> AllTag { get; set; }
    }
}
