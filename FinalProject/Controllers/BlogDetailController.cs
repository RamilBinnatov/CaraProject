using FinalProject.Data;
using FinalProject.Models;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Controllers
{
    public class BlogDetailController : Controller
    {
        #region readonly
        private readonly AppDbContext _context;
        #endregion

        #region Constructor
        public BlogDetailController(AppDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index(int? id)
        {
            Blog blog = await _context.Blogs.Where(m => !m.IsDeleted && m.Id == id)
                .Include(m => m.BlogImage)
                .Include(m => m.BlogCategory)
                .Include(m => m.BlogTags)
                .FirstOrDefaultAsync();
            IEnumerable<Blog> RecentPosts = await _context.Blogs
                .Where(m => !m.IsDeleted)
                .Take(3)    
                .OrderByDescending(m => m.Id)
                .Include(m => m.BlogImage)
                .ToListAsync();

            IEnumerable<Tag> AllTag = await _context.Tags.Where(m => !m.IsDeleted).ToListAsync();

            if (blog == null) return NotFound();

            List<BlogTag> blogTags = await _context.BlogTags.Where(m => m.BlogId == id).ToListAsync();
            List<Tag> tags = new List<Tag>();
            foreach (var dbTags in blogTags)
            {
                Tag dbTag = await _context.Tags.Where(m => m.Id == dbTags.TagId).FirstOrDefaultAsync();
                tags.Add(dbTag);
            }

            BlogDetailsVM blogDetailsVM = new BlogDetailsVM
            {
                Id = blog.Id,
                Title = blog.Title,
                Content = blog.Content,
                blogImages = blog.BlogImage,
                RecentPosts = RecentPosts,
                MainImage = blog.BlogImage.FirstOrDefault(m => m.IsMain)?.Image,
                Tags = tags,
                AllTag = AllTag,
                Createdate = blog.CreateDate,
                Creator = blog.Creator,

            };

            return View(blogDetailsVM);
        }
        #endregion
    }
}
