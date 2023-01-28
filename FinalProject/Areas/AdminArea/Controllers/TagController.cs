using FinalProject.Data;
using FinalProject.Helpers;
using FinalProject.Helpers.Enums;
using FinalProject.Models;
using FinalProject.ViewModels.Tag;
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
    public class TagController : Controller
    {
        #region readonly
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        #endregion

        #region Constructor
        public TagController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index(int page = 1, int take = 5)
        {
            List<Tag> tags = await _context.Tags
                .Where(m => !m.IsDeleted)
                .Skip((page * take) - take)
                .Take(take)
                .ToListAsync();

            ViewBag.take = take;

            List<TagListVM> mapDatas = GetMapDatas(tags);

            int count = await GetPageCount(take);

            Paginate<TagListVM> result = new Paginate<TagListVM>(mapDatas, page, count);

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
        public async Task<IActionResult> Create(TagCreateVM tag)
        {
            if (!ModelState.IsValid) return View();


            Tag newTag = new Tag
            {
                Name = tag.Name,
            };

            await _context.Tags.AddAsync(newTag);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            Tag tag = await _context.Tags.FirstOrDefaultAsync(m => m.Id == id);

            tag.IsDeleted = true;

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

                Tag tag = await _context.Tags.FirstOrDefaultAsync(m => m.Id == id);

                if (tag is null) return NotFound();

                return View(new TagEditVM
                {
                    Name = tag.Name,

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
        public async Task<IActionResult> Update(int id, TagEditVM tag)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(tag);
                }
                Tag dbTag = await GetByIdAsync(id);


                dbTag.Name = tag.Name;

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
        private async Task<Tag> GetByIdAsync(int id)
        {
            return await _context.Tags.FindAsync(id);
        }

        private List<TagListVM> GetMapDatas(List<Tag> tags)
        {
            List<TagListVM> tagListVMs = new List<TagListVM>();

            foreach (var item in tags)
            {
                TagListVM newTag = new TagListVM
                {
                    Id = item.Id,
                    Name = item.Name,
                };

                tagListVMs.Add(newTag);
            }

            return tagListVMs;
        }

        private async Task<int> GetPageCount(int take)
        {
            int tagCount = await _context.Tags.Where(m => !m.IsDeleted).CountAsync();

            return (int)Math.Ceiling((decimal)tagCount / take);
        }

        #endregion
    }
}
