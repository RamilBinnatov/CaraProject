using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class Blog : BaseEntity
    {
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public int BlogCategoryId { get; set; }
        public BlogCategory BlogCategory { get; set; }
        public ICollection<BlogImage> BlogImage { get; set; }
        public ICollection<BlogTag> BlogTags { get; set; }

    }
}
