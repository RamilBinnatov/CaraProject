using FinalProject.Data;
using FinalProject.Models;
using FinalProject.Services;
using FinalProject.ViewModels;
using FinalProject.ViewModels.Basket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly LayoutService _layoutService;
        private readonly AppDbContext _context;

        public FooterViewComponent(LayoutService layoutService, AppDbContext context)
        {
            _layoutService = layoutService;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Dictionary<string, string> datas = await _layoutService.GetDatasFromSetting();
            List<BasketDetailVM> basketDetailList = new List<BasketDetailVM>();
            IEnumerable<Social> socials = await _context.Socials.Where(m => !m.IsDeleted).ToListAsync();

            if (Request.Cookies["basket"] != null)
            {
                List<BasketVM> basketItems = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);

                foreach (var item in basketItems)
                {
                    Product product = await _context.Products
                        .Where(m => m.Id == item.Id && m.IsDeleted == false)
                        .Include(m => m.ProductImages).FirstOrDefaultAsync();

                    BasketDetailVM basketModel = new BasketDetailVM
                    {
                        Id = product.Id,
                        Title = product.Title,
                        Image = product.ProductImages.Where(m => m.IsMain)?.FirstOrDefault().Image,
                        Price = product.Price,
                        Count = item.Count,
                        Total = product.Price * item.Count,
                    };
                    basketDetailList.Add(basketModel);
                }

                FooterVM footerVM = new FooterVM
                {
                    Socials = socials,
                    BasketProduct = basketDetailList,
                };
                return await Task.FromResult(View(footerVM));
            }
            else
            {
                FooterVM footerVM = new FooterVM
                {
                    Socials = socials,
                    BasketProduct = basketDetailList,
                };
                return await Task.FromResult(View(footerVM));
            }
        }


    }
}
