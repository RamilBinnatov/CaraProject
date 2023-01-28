using FinalProject.Data;
using FinalProject.Models;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Controllers
{
    public class AboutController : Controller
    {

        #region Readonly
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        #endregion

        #region Constructor
        public AboutController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index()
        {
            AboutUs aboutUs = await _context.AboutUs
                .Where(m => !m.IsDeleted)
                .FirstOrDefaultAsync();
            IEnumerable<PageHeader> pageHeaders = await _context.PageHeaders
                .Where(m => !m.IsDeleted)
                .ToListAsync();




            AboutVM model = new AboutVM
            {
                 PageHeaders = pageHeaders,
                 AboutUs = aboutUs

            };

            return View(model);
        }
        #endregion

    }
}
