using FinalProject.Data;
using FinalProject.Helpers;
using FinalProject.Helpers.Enums;
using FinalProject.Models;
using FinalProject.ViewModels.Social;
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

    [Area("Adminarea")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class SocialController : Controller
    {
        #region readonly
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        #endregion

        #region Constructor
        public SocialController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index(int page = 1, int take = 5)
        {
            List<Social> socials = await _context.Socials
                .Where(m => !m.IsDeleted)
                .Skip((page * take) - take)
                .Take(take)
                .ToListAsync();

            ViewBag.take = take;

            List<SocialListVM> mapDatas = GetMapDatas(socials);

            int count = await GetPageCount(take);

            Paginate<SocialListVM> result = new Paginate<SocialListVM>(mapDatas, page, count);

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
        public async Task<IActionResult> Create(SocialCreateVM social)
        {
            if (!ModelState.IsValid) return View();


            Social newSocial = new Social
            {
                Name = social.Name,
                URL = social.URL,
                Logo = social.Icon
            };

            await _context.Socials.AddAsync(newSocial);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            Social social = await _context.Socials.FirstOrDefaultAsync(m => m.Id == id);

            social.IsDeleted = true;

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

                Social social = await _context.Socials.FirstOrDefaultAsync(m => m.Id == id);

                if (social is null) return NotFound();

                return View(new SocialEditVM
                {
                    Name = social.Name,
                    URL = social.URL,
                    Icon = social.Logo

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
        public async Task<IActionResult> Update(int id, SocialEditVM social)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(social);
                }
                Social dbSocial = await GetByIdAsync(id);


                dbSocial.Name = social.Name;
                dbSocial.Logo = social.Icon;
                dbSocial.URL = social.URL;

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
        private async Task<Social> GetByIdAsync(int id)
        {
            return await _context.Socials.FindAsync(id);
        }

        private List<SocialListVM> GetMapDatas(List<Social> socials)
        {
            List<SocialListVM> socialListVMs = new List<SocialListVM>();

            foreach (var item in socials)
            {
                SocialListVM newSocial = new SocialListVM
                {
                    Id = item.Id,
                    Name = item.Name,
                    URL = item.URL,
                    Icon = item.Logo,

                };

                socialListVMs.Add(newSocial);
            }

            return socialListVMs;
        }

        private async Task<int> GetPageCount(int take)
        {
            int socialCount = await _context.Socials.Where(m => !m.IsDeleted).CountAsync();

            return (int)Math.Ceiling((decimal)socialCount / take);
        }

        #endregion
    }
}
