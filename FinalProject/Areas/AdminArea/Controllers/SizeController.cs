using FinalProject.Data;
using FinalProject.Helpers;
using FinalProject.Helpers.Enums;
using FinalProject.Models;
using FinalProject.ViewModels.Size;
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
    public class SizeController : Controller
    {
        #region readonly
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        #endregion

        #region Constructor
        public SizeController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index(int page = 1, int take = 5)
        {
            List<Size> sizes = await _context.Sizes
                .Where(m => !m.IsDeleted)
                .Skip((page * take) - take)
                .Take(take)
                .ToListAsync();

            ViewBag.take = take;

            List<SizeListVM> mapDatas = GetMapDatas(sizes);

            int count = await GetPageCount(take);

            Paginate<SizeListVM> result = new Paginate<SizeListVM>(mapDatas, page, count);

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
        public async Task<IActionResult> Create(SizeCreateVM size)
        {
            if (!ModelState.IsValid) return View();


            Size newSize = new Size
            {
                Name = size.Name,
            };

            await _context.Sizes.AddAsync(newSize);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            Size size = await _context.Sizes.FirstOrDefaultAsync(m => m.Id == id);

            size.IsDeleted = true;

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

                Size size = await _context.Sizes.FirstOrDefaultAsync(m => m.Id == id);

                if (size is null) return NotFound();

                return View(new SizeEditVM
                {
                    Name = size.Name,

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
        public async Task<IActionResult> Update(int id, SizeEditVM size)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(size);
                }
                Size dbSize = await GetByIdAsync(id);


                dbSize.Name = size.Name;

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
        private async Task<Size> GetByIdAsync(int id)
        {
            return await _context.Sizes.FindAsync(id);
        }

        private List<SizeListVM> GetMapDatas(List<Size> sizes)
        {
            List<SizeListVM> sizeListVMs = new List<SizeListVM>();

            foreach (var item in sizes)
            {
                SizeListVM newSize = new SizeListVM
                {
                    Id = item.Id,
                    Name = item.Name,
                };

                sizeListVMs.Add(newSize);
            }

            return sizeListVMs;
        }

        private async Task<int> GetPageCount(int take)
        {
            int sizeCount = await _context.Sizes.Where(m => !m.IsDeleted).CountAsync();

            return (int)Math.Ceiling((decimal)sizeCount / take);
        }

        #endregion
    }
}
