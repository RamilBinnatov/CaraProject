using FinalProject.Data;
using FinalProject.Helpers;
using FinalProject.Helpers.Enums;
using FinalProject.Models;
using FinalProject.ViewModels.Poster;
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
    public class PosterController : Controller
    {
        #region readonly
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        #endregion

        #region Constructor
        public PosterController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index(int page = 1, int take = 5)
        {
            List<Poster> posters = await _context.Posters
                .Where(m => !m.IsDeleted)
                .Skip((page * take) - take)
                .Take(take)
                .ToListAsync();

            ViewBag.take = take;

            List<PosterListVM> mapDatas = GetMapDatas(posters);

            int count = await GetPageCount(take);

            Paginate<PosterListVM> result = new Paginate<PosterListVM>(mapDatas, page, count);

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

                Poster poster = await _context.Posters.FirstOrDefaultAsync(m => m.Id == id);

                if (poster is null) return NotFound();

                return View(new PosterEditVM
                {
                    Title = poster.Title,
                    Header = poster.Header,
                    Description = poster.Descrption,
                    Image = poster.Image,
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
        public async Task<IActionResult> Update(int id, PosterEditVM poster)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(poster);
                }
                Poster dbPoster = await GetByIdAsync(id);
                if (poster.Photo != null)
                {
                    if (!poster.Photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image type");
                        return View();
                    }

                    if (!poster.Photo.CheckFileSize(20000))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image size");
                        return View();
                    }
                    string fileName = Guid.NewGuid().ToString() + "_" + poster.Photo.FileName;
                    Poster posterDb = await _context.Posters.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
                    if (posterDb is null) return NotFound();

                    if (posterDb.Image == poster.Image)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    string path = Helper.GetFilePath(_env.WebRootPath, "assets/img/Hero", fileName);
                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        await poster.Photo.CopyToAsync(stream);
                    }

                    dbPoster.Image = fileName;

                }

                dbPoster.Title = poster.Title;
                dbPoster.Header = poster.Header;
                dbPoster.Descrption = poster.Description;

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
        public async Task<IActionResult> Create(PosterCreateVM poster)
        {
            if (!ModelState.IsValid) return View();

            if (!poster.Photo.CheckFileType("image/"))
            {
                ModelState.AddModelError("Photo", "Please choose correct image type");
                return View();
            }

            if (!poster.Photo.CheckFileSize(200000))
            {
                ModelState.AddModelError("Photo", "Please choose correct image size");
                return View();
            }

            string fileName = Guid.NewGuid().ToString() + "_" + poster.Photo.FileName;

            string path = Helper.GetFilePath(_env.WebRootPath, "assets/img/hero", fileName);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await poster.Photo.CopyToAsync(stream);
            }

            Poster newPoster = new Poster
            {
                Title = poster.Title,
                Header = poster.Header,
                Descrption = poster.Description,
                Image = fileName,
            };

            await _context.Posters.AddAsync(newPoster);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int id)
        {
            Poster poster = await GetByIdAsync(id);

            if (poster == null) return NotFound();

            string path = Helper.GetFilePath(_env.WebRootPath, "img", poster.Image);


            Helper.DeleteFile(path);

            _context.Posters.Remove(poster);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region SetStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetStatus(int id)
        {
            List<Poster> dbPoster = await _context.Posters.Where(m => m.isActive).ToListAsync();

            if (dbPoster.Count < 2)
            {
                Poster poster = await _context.Posters.FirstOrDefaultAsync(m => m.Id == id);

                if (poster is null) return NotFound();

                if (poster.isActive)
                {
                    poster.isActive = false;
                }
                else
                {
                    poster.isActive = true;
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                Poster poster = await _context.Posters.FirstOrDefaultAsync(m => m.Id == id);
                if (poster.isActive)
                {
                    poster.isActive = false;
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }


        }
        #endregion

        #region Services
        private async Task<Poster> GetByIdAsync(int id)
        {
            return await _context.Posters.FindAsync(id);
        }

        private List<PosterListVM> GetMapDatas(List<Poster> posters)
        {
            List<PosterListVM> posterListVMs = new List<PosterListVM>();

            foreach (var item in posters)
            {
                PosterListVM posterListVM = new PosterListVM
                {
                    Id = item.Id,
                    Description = item.Descrption,
                    Title = item.Title,
                    Header = item.Header,
                    IsActive = item.isActive,
                    Image = item.Image
                };

                posterListVMs.Add(posterListVM);
            }

            return posterListVMs;
        }

        private async Task<int> GetPageCount(int take)
        {
            int posterCount = await _context.Posters.Where(m => !m.IsDeleted).CountAsync();

            return (int)Math.Ceiling((decimal)posterCount / take);
        }
        #endregion
    }
}
