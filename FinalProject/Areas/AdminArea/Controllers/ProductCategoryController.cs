using FinalProject.Data;
using FinalProject.Helpers;
using FinalProject.Helpers.Enums;
using FinalProject.Models;
using FinalProject.ViewModels.ProductCategory;
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
    public class ProductCategoryController : Controller
    {
        #region readonly
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        #endregion

        #region Constructor
        public ProductCategoryController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index(int page = 1, int take = 5)
        {
            List<ProductCategory> categories = await _context.ProductCategories
                .Where(m => !m.IsDeleted)
                .Skip((page * take) - take)
                .Take(take)
                .ToListAsync();

            ViewBag.take = take;

            List<ProductCategoryListVM> mapDatas = GetMapDatas(categories);

            int count = await GetPageCount(take);

            Paginate<ProductCategoryListVM> result = new Paginate<ProductCategoryListVM>(mapDatas, page, count);

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
        public async Task<IActionResult> Create(ProductCategoryCreateVM categories)
        {
            if (!ModelState.IsValid) return View();


            ProductCategory newCategory = new ProductCategory
            {
                Name = categories.Name,
            };

            await _context.ProductCategories.AddAsync(newCategory);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            ProductCategory category = await _context.ProductCategories.FirstOrDefaultAsync(m => m.Id == id);

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

                ProductCategory category = await _context.ProductCategories.FirstOrDefaultAsync(m => m.Id == id);

                if (category is null) return NotFound();

                return View(new ProductCategoryEditVM
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
        public async Task<IActionResult> Update(int id, ProductCategoryEditVM categories)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(categories);
                }
                ProductCategory dbCategory = await GetByIdAsync(id);


                dbCategory.Name = categories.Name;

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
        private async Task<ProductCategory> GetByIdAsync(int id)
        {
            return await _context.ProductCategories.FindAsync(id);
        }

        private List<ProductCategoryListVM> GetMapDatas(List<ProductCategory> categories)
        {
            List<ProductCategoryListVM> productCategoryListVMs = new List<ProductCategoryListVM>();

            foreach (var item in categories)
            {
                ProductCategoryListVM newCategory = new ProductCategoryListVM
                {
                    Id = item.Id,
                    Name = item.Name,
                };

                productCategoryListVMs.Add(newCategory);
            }

            return productCategoryListVMs;
        }

        private async Task<int> GetPageCount(int take)
        {
            int categoryCount = await _context.ProductCategories.Where(m => !m.IsDeleted).CountAsync();

            return (int)Math.Ceiling((decimal)categoryCount / take);
        }

        #endregion
    }
}
