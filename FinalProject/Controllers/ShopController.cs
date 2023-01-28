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
    public class ShopController : Controller
    {
        #region readonly
        private readonly AppDbContext _context;
        private readonly LayoutService _layoutService;
        #endregion

        #region constructor
        public ShopController(AppDbContext context, LayoutService layoutService)
        {
            _context = context;
            _layoutService = layoutService;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index(int page = 1, int take = 8)
        {
            List<Product> products = await _context.Products
                .Where(m => !m.IsDeleted)
                .Include(m=>m.Brand)
                .Include(m=>m.ProductImages)
                .Skip((page * take) - take)
                .Take(take)
                .ToListAsync();

            ViewBag.take = take;

            IEnumerable<PageHeader> pageHeaders = await _context.PageHeaders.Where(m => !m.IsDeleted && m.Page == "Shop").ToListAsync();

            List<ShopVM> shopVMs = new List<ShopVM>();

            ShopVM model = new ShopVM
            {
                Products = products.ToList(),
                PageHeaders = pageHeaders,
            };

            shopVMs.Add(model);

            int count = await GetPageCount(take);

            Paginate<ShopVM> result = new Paginate<ShopVM>(shopVMs, page, count);

            return View(result);
        }
        #endregion

        #region Services
        public async Task<int> GetPageCount(int take)
        {
            int productCount = await _context.Products.Where(m => !m.IsDeleted).CountAsync();
            return (int)Math.Ceiling((decimal)productCount / take);
        }
        #endregion
    }
}
