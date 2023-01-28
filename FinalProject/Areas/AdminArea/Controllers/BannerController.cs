using FinalProject.Data;
using FinalProject.Helpers;
using FinalProject.Helpers.Enums;
using FinalProject.Models;
using FinalProject.ViewModels.Banner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class BannerController : Controller
    {
        #region readonly
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        #endregion

        #region Constructor
        public BannerController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index(int page = 1, int take = 5)
        {
            List<Banner> banners = await _context.Banners
                .Where(m => !m.IsDeleted)
                .Skip((page * take) - take)
                .Take(take)
                .ToListAsync();

            ViewBag.take = take;

            List<BannerListVM> mapDatas = GetMapDatas(banners);

            int count = await GetPageCount(take);

            Paginate<BannerListVM> result = new Paginate<BannerListVM>(mapDatas, page, count);

            return View(result);
        }
        #endregion

        #region Edit
        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            try
            {
                if (id is null) return BadRequest();

                Banner banner = await _context.Banners.FirstOrDefaultAsync(m => m.Id == id);

                if (banner is null) return NotFound();

                return View(new BannerEditVM
                {
                    Header = banner.Header,
                    Description = banner.Description,
                    Image = banner.Image,
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
        public async Task<IActionResult> Update(int id, BannerEditVM banner)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(banner);
                }
                Banner dbBanner = await GetByIdAsync(id);
                if (banner.Photo != null)
                {
                    if (!banner.Photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image type");
                        return View();
                    }

                    if (!banner.Photo.CheckFileSize(20000))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image size");
                        return View();
                    }
                    string fileName = Guid.NewGuid().ToString() + "_" + banner.Photo.FileName;
                    Banner bannerDb = await _context.Banners.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
                    if (bannerDb is null) return NotFound();

                    if (bannerDb.Image == banner.Image)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    string path = Helper.GetFilePath(_env.WebRootPath, "assets/img/Hero", fileName);
                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        await banner.Photo.CopyToAsync(stream);
                    }

                    dbBanner.Image = fileName;

                }

                dbBanner.Header = banner.Header;
                dbBanner.Description = banner.Description;

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

        #region Create

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BannerCreateVM banner)
        {
            if (!ModelState.IsValid) return View();

            if (!banner.Photo.CheckFileType("image/"))
            {
                ModelState.AddModelError("Photo", "Please choose correct image type");
                return View();
            }

            if (!banner.Photo.CheckFileSize(200000))
            {
                ModelState.AddModelError("Photo", "Please choose correct image size");
                return View();
            }

            string fileName = Guid.NewGuid().ToString() + "_" + banner.Photo.FileName;

            string path = Helper.GetFilePath(_env.WebRootPath, "assets/img/hero", fileName);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await banner.Photo.CopyToAsync(stream);
            }

            Banner newBanner = new Banner
            {
                Header = banner.Header,
                Description = banner.Description,
                Image = fileName,
            };

            await _context.Banners.AddAsync(newBanner);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int id)
        {
            Banner banner = await GetByIdAsync(id);

            if (banner == null) return NotFound();

            string path = Helper.GetFilePath(_env.WebRootPath, "img", banner.Image);


            Helper.DeleteFile(path);

            _context.Banners.Remove(banner);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region SetStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetStatus(int id)
        {
            List<Banner> dbBanner = await _context.Banners.Where(m => m.isActive).ToListAsync();

            if (dbBanner.Count < 3)
            {
                Banner banner = await _context.Banners.FirstOrDefaultAsync(m => m.Id == id);

                if (banner is null) return NotFound();

                if (banner.isActive)
                {
                    banner.isActive = false;
                }
                else
                {
                    banner.isActive = true;
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                Banner banner = await _context.Banners.FirstOrDefaultAsync(m => m.Id == id);
                if (banner.isActive)
                {
                    banner.isActive = false;
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }


        }
        #endregion

        #region Services
        private async Task<Banner> GetByIdAsync(int id)
        {
            return await _context.Banners.FindAsync(id);
        }

        private List<BannerListVM> GetMapDatas(List<Banner> banners)
        {
            List<BannerListVM> bannerListVMs = new List<BannerListVM>();

            foreach (var item in banners)
            {
                BannerListVM newBanner = new BannerListVM
                {
                    Id = item.Id,
                    Description = item.Description,
                    Header = item.Header,
                    IsActive = item.isActive,
                    Image = item.Image
                };

                bannerListVMs.Add(newBanner);
            }

            return bannerListVMs;
        }

        private async Task<int> GetPageCount(int take)
        {
            int bannerCount = await _context.Banners.Where(m => !m.IsDeleted).CountAsync();

            return (int)Math.Ceiling((decimal)bannerCount / take);
        }

        #endregion

    }
}
