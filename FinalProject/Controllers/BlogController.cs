using FinalProject.Data;
using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.Services;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Controllers
{
    public class BlogController : Controller
    {
        #region readonly
        private readonly AppDbContext _context;
        private readonly LayoutService _layoutService;
        #endregion

        #region Constructor
        public BlogController(AppDbContext context, LayoutService layoutService)

        {
            _context = context;
            _layoutService = layoutService;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index(int page = 1, int take = 6)
        {
            List<Blog> blogs = await _context.Blogs
                .Where(m => !m.IsDeleted)
                .Include(m=>m.BlogImage)
                .Skip((page * take) - take)
                .Take(take)
                .ToListAsync();

            IEnumerable<PageHeader> pageHeaders = await _context.PageHeaders.Where(m => !m.IsDeleted && m.Page == "Blog").ToListAsync();

            ViewBag.take = take;


            List<BlogVM> blogVMs = new List<BlogVM>();

            BlogVM model = new BlogVM
            {
                Blogs = blogs.ToList(),
                PageHeaders = pageHeaders,
                
            };

            blogVMs.Add(model);

            int count = await GetPageCount(take);

            Paginate<BlogVM> result = new Paginate<BlogVM>(blogVMs, page, count);

            return View(result);
        }
        #endregion

        #region Services
        public async Task<int> GetPageCount(int take)
        {
            int blogCount = await _context.Blogs.Where(m => !m.IsDeleted).CountAsync();
            return (int)Math.Ceiling((decimal)blogCount / take);
        }
        #endregion
    }
}
