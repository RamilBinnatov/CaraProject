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
    public class ShopDetailController : Controller
    {
        #region readonly
        private readonly AppDbContext _context;
        #endregion

        #region Constructor
        public ShopDetailController(AppDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Product product = await _context.Products
                .Where(m => !m.IsDeleted && m.Id == id)
                .Include(m => m.ProductImages)
                .Include(m => m.ProductCategory)
                .Include(m => m.Brand)
                .Include(m => m.Product_Sizes)
                .FirstOrDefaultAsync();
            IEnumerable<Product> products = await _context.Products
                .Where(m => !m.IsDeleted)
                .Where(m => m.ProductCategoryId == product.ProductCategoryId)
                .Take(4)
                .OrderByDescending(m => m.Id)
                .Include(m => m.ProductImages)
                .Include(m => m.Brand)
                .ToListAsync();

            if (product == null)
            {
                return NotFound();
            }

            List<Product_Size> product_Sizes = await _context.Product_Size.Where(m => m.ProductId == id).ToListAsync();
            List<Size> sizes = new List<Size>();
            foreach (var size in product_Sizes)
            {
                Size dbSize = await _context.Sizes.Where(m => m.Id == size.SizeId).FirstOrDefaultAsync();
                sizes.Add(dbSize);
            }

            ShopDetailVM productDetailVM = new ShopDetailVM
            {
                Id = product.Id,
                Title = product.Title,
                Price = product.Price,
                Description = product.Description,
                CategoryName = product.ProductCategory.Name,
                ProductImages = product.ProductImages,
                FeaturedProducts = products,
                MainImage = product.ProductImages.FirstOrDefault(m => m.IsMain)?.Image,
                Sizes = sizes
            };

            return View(productDetailVM);
        }
        #endregion
    }
}
