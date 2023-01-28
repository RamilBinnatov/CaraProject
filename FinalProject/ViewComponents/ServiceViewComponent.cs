using FinalProject.Data;
using FinalProject.Models;
using FinalProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewComponents
{
    
    public class ServiceViewComponent : ViewComponent
    {
        private readonly LayoutService _layoutService;
        private readonly AppDbContext _context;

        public ServiceViewComponent(LayoutService layoutService, AppDbContext context)
        {
            _layoutService = layoutService;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Dictionary<string, string> datas = await _layoutService.GetDatasFromSetting();

            IEnumerable<Service> services = await _context.Services.Where(m => !m.IsDeleted).ToListAsync();

            return await Task.FromResult(View(services));
        }


    }
}
