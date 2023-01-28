using FinalProject.Data;
using FinalProject.Helpers;
using FinalProject.Helpers.Enums;
using FinalProject.Models;
using FinalProject.ViewModels.Hero;
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
    public class HeroController : Controller
    {
        #region readonly
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        #endregion

        #region Constructor
        public HeroController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index(int page = 1, int take = 5)
        {
            List<Hero> heroes = await _context.Heroes
                .Where(m => !m.IsDeleted)
                .Skip((page * take) - take)
                .Take(take)
                .ToListAsync();

            ViewBag.take = take;

            List<HeroListVM> mapDatas = GetMapDatas(heroes);

            int count = await GetPageCount(take);

            Paginate<HeroListVM> result = new Paginate<HeroListVM>(mapDatas, page, count);

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

                Hero hero = await _context.Heroes.FirstOrDefaultAsync(m => m.Id == id);

                if (hero is null) return NotFound();

                return View(new HeroEditVM
                {
                    Title = hero.Title,
                    Content = hero.Content,
                    Header = hero.Header,
                    Description = hero.Description,
                    Image = hero.Image,
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
        public async Task<IActionResult> Update(int id, HeroEditVM hero)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(hero);
                }
                Hero dbHero = await GetByIdAsync(id);
                if (hero.Photo != null)
                {
                    if (!hero.Photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image type");
                        return View();
                    }

                    if (!hero.Photo.CheckFileSize(20000))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image size");
                        return View();
                    }
                    string fileName = Guid.NewGuid().ToString() + "_" + hero.Photo.FileName;
                    Hero heroDb = await _context.Heroes.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
                    if (heroDb is null) return NotFound();

                    if (heroDb.Image == hero.Image)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    string path = Helper.GetFilePath(_env.WebRootPath, "assets/img/Hero", fileName);
                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        await hero.Photo.CopyToAsync(stream);
                    }

                    dbHero.Image = fileName;

                }

                dbHero.Title = hero.Title;
                dbHero.Content = hero.Content;
                dbHero.Header = hero.Header;
                dbHero.Description = hero.Description;

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
        public async Task<IActionResult> Create(HeroCreateVM hero)
        {
            if (!ModelState.IsValid) return View();

            if (!hero.Photo.CheckFileType("image/"))
            {
                ModelState.AddModelError("Photo", "Please choose correct image type");
                return View();
            }

            if (!hero.Photo.CheckFileSize(200000))
            {
                ModelState.AddModelError("Photo", "Please choose correct image size");
                return View();
            }

            string fileName = Guid.NewGuid().ToString() + "_" + hero.Photo.FileName;

            string path = Helper.GetFilePath(_env.WebRootPath, "assets/img/hero", fileName);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await hero.Photo.CopyToAsync(stream);
            }

            Hero newHero = new Hero
            {
                Title = hero.Title,
                Content = hero.Content,
                Header = hero.Header,
                Description = hero.Description,
                Image = fileName,
            };

            await _context.Heroes.AddAsync(newHero);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int id)
        {
            Hero hero = await GetByIdAsync(id);

            if (hero == null) return NotFound();

            string path = Helper.GetFilePath(_env.WebRootPath, "img", hero.Image);


            Helper.DeleteFile(path);

            _context.Heroes.Remove(hero);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region SetStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetStatus(int id)
        {
            List<Hero> dbHeros = await _context.Heroes.Where(m => m.IsActive).ToListAsync();

            if (dbHeros.Count < 1)
            {
                Hero hero = await _context.Heroes.FirstOrDefaultAsync(m => m.Id == id);

                if (hero is null) return NotFound();

                if (hero.IsActive)
                {
                    hero.IsActive = false;
                }
                else
                {
                    hero.IsActive = true;
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                Hero hero = await _context.Heroes.FirstOrDefaultAsync(m => m.Id == id);
                if (hero.IsActive)
                {
                    hero.IsActive = false;
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }


        }
        #endregion

        #region Services
        private async Task<Hero> GetByIdAsync(int id)
        {
            return await _context.Heroes.FindAsync(id);
        }

        private List<HeroListVM> GetMapDatas(List<Hero> heroes)
        {
            List<HeroListVM> heroListVMs = new List<HeroListVM>();

            foreach (var item in heroes)
            {
                HeroListVM heroListVM = new HeroListVM
                {
                    Id = item.Id,
                    Description = item.Description,
                    Title = item.Title,
                    Header = item.Description,
                    Content = item.Content,
                    IsActive = item.IsActive,
                    Image = item.Image
                };

                heroListVMs.Add(heroListVM);
            }

            return heroListVMs;
        }

        private async Task<int> GetPageCount(int take)
        {
            int heroCount = await _context.Heroes.Where(m => !m.IsDeleted).CountAsync();

            return (int)Math.Ceiling((decimal)heroCount / take);
        }
        #endregion
    }
}
