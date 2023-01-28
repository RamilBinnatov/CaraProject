using FinalProject.Data;
using FinalProject.Helpers;
using FinalProject.Helpers.Enums;
using FinalProject.Models;
using FinalProject.ViewModels.BlogCategory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Areas.AdminArea.Controllers
{

    [Area("AdminArea")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class BlogCategoryController : Controller
    {
        #region readonly
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        #endregion

        #region Constructor
        public BlogCategoryController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index(int page = 1, int take = 5)
        {
            List<BlogCategory> categories = await _context.BlogCategories
                .Where(m => !m.IsDeleted)
                .Skip((page * take) - take)
                .Take(take)
                .ToListAsync();

            ViewBag.take = take;

            List<BlogCategoryListVM> mapDatas = GetMapDatas(categories);

            int count = await GetPageCount(take);

            Paginate<BlogCategoryListVM> result = new Paginate<BlogCategoryListVM>(mapDatas, page, count);

            return View(result);
        }
        #endregion

        #region Create

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogCategoryCreateVM categories)
        {
            if (!ModelState.IsValid) return View();


            BlogCategory newCategory = new BlogCategory
            {
                Name = categories.Name,
            };

            await _context.BlogCategories.AddAsync(newCategory);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            BlogCategory category = await _context.BlogCategories.FirstOrDefaultAsync(m => m.Id == id);

            category.IsDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Edit
        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            try
            {
                if (id is null) return BadRequest();

                BlogCategory category = await _context.BlogCategories.FirstOrDefaultAsync(m => m.Id == id);

                if (category is null) return NotFound();

                return View(new BlogCategoryEditVM
                {
                    Name = category.Name,
                });

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, BlogCategoryEditVM categoies)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(categoies);
                }
                BlogCategory dbCategory = await GetByIdAsync(id);


                dbCategory.Name = categoies.Name;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }
        }
        #endregion

        #region Services
        private async Task<BlogCategory> GetByIdAsync(int id)
        {
            return await _context.BlogCategories.FindAsync(id);
        }

        private List<BlogCategoryListVM> GetMapDatas(List<BlogCategory> categories)
        {
            List<BlogCategoryListVM> blogCategoryListVMs = new List<BlogCategoryListVM>();

            foreach (var item in categories)
            {
                BlogCategoryListVM newCategory = new BlogCategoryListVM
                {
                    Id = item.Id,
                    Name = item.Name,
                };

                blogCategoryListVMs.Add(newCategory);
            }

            return blogCategoryListVMs;
        }

        private async Task<int> GetPageCount(int take)
        {
            int categoryCount = await _context.BlogCategories.Where(m => !m.IsDeleted).CountAsync();

            return (int)Math.Ceiling((decimal)categoryCount / take);
        }

        #endregion
    }
}
