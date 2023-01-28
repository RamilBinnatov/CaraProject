using FinalProject.Data;
using FinalProject.Models;
using FinalProject.Services;
using FinalProject.ViewModels;
using FinalProject.ViewModels.Basket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Controllers
{
    public class HomeController : Controller
    {
        #region Readonly
        private readonly AppDbContext _context;
        private readonly LayoutService _layoutService;
        #endregion

        #region Constructor
        public HomeController(AppDbContext context, LayoutService layoutService)
        {
            _context = context;
            _layoutService = layoutService;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index()
        {
            Dictionary<string, string> settingDatas = await _layoutService.GetDatasFromSetting();

            IEnumerable<Banner> banners = await _context.Banners
                .Where(m => !m.IsDeleted)
                .Where(m => m.isActive)
                .ToListAsync();
            IEnumerable<Board> boards = await _context.Boards
                .Where(m => !m.IsDeleted)
                .Where(m => m.isActive)
                .ToListAsync();
            IEnumerable<Poster> posters = await _context.Posters
                .Where(m => !m.IsDeleted)
                .Where(m => m.isActive)
                .ToListAsync();
            IEnumerable<Hero> heroes = await _context.Heroes
                .Where(m => !m.IsDeleted)
                .Where(m => m.IsActive)
                .ToListAsync();
            IEnumerable<Product> bestSelling = await _context.Products
                .Where(m =>! m.IsDeleted)
                .OrderByDescending(m => m.SellingCount)
                .Include(m => m.ProductCategory)
                .Include(m => m.ProductImages)
                .Include(m => m.Brand)
                .Take(4)
                .ToListAsync();
            IEnumerable<Product> newArrivals = await _context.Products
                .Where(m => !m.IsDeleted)
                .OrderByDescending(m => m.Id)
                .Include(m => m.ProductCategory)
                .Include(m => m.ProductImages)
                .Include(m => m.Brand)
                .Take(4)
                .ToListAsync();

            HomeVM model = new HomeVM
            {
                BestSelling = bestSelling,
                Banners = banners,
                Posters = posters,
                Boards = boards,
                NewArrivals = newArrivals,
                Heroes = heroes,
            };

            return View(model);
        }
        #endregion

        #region Search
        public IActionResult Search(string search)
        {
            List<Product> searchName = _context.Products.Where(s => !s.IsDeleted && s.Title.Trim().Contains(search.Trim())).Include(m => m.ProductImages).ToList();
            return PartialView("_Search", searchName);
        }
        #endregion
    }
}
