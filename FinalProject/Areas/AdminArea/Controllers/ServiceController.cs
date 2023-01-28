using FinalProject.Data;
using FinalProject.Helpers;
using FinalProject.Helpers.Enums;
using FinalProject.Models;
using FinalProject.ViewModels.Service;
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
    public class ServiceController : Controller
    {
        #region readonly
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        #endregion

        #region Constructor
        public ServiceController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index(int page = 1, int take = 5)
        {
            List<Service> services = await _context.Services
                .Where(m => !m.IsDeleted)
                .Skip((page * take) - take)
                .Take(take)
                .ToListAsync();

            ViewBag.take = take;

            List<ServiceListVM> mapDatas = GetMapDatas(services);

            int count = await GetPageCount(take);

            Paginate<ServiceListVM> result = new Paginate<ServiceListVM>(mapDatas, page, count);

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

                Service service = await _context.Services.FirstOrDefaultAsync(m => m.Id == id);

                if (service is null) return NotFound();

                return View(new ServiceEditVM
                {
                    Title = service.Title,
                    Image = service.Image,
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
        public async Task<IActionResult> Update(int id, ServiceEditVM service)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(service);
                }
                Service dbService = await GetByIdAsync(id);
                if (service.Photo != null)
                {
                    if (!service.Photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image type");
                        return View();
                    }

                    if (!service.Photo.CheckFileSize(20000))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image size");
                        return View();
                    }
                    string fileName = Guid.NewGuid().ToString() + "_" + service.Photo.FileName;
                    Service serviceDb = await _context.Services.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
                    if (serviceDb is null) return NotFound();

                    if (serviceDb.Image == service.Image)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    string path = Helper.GetFilePath(_env.WebRootPath, "assets/img/features", fileName);
                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        await service.Photo.CopyToAsync(stream);
                    }

                    dbService.Image = fileName;

                }

                dbService.Title = service.Title;

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
        public async Task<IActionResult> Create(ServiceCreateVM service)
        {
            if (!ModelState.IsValid) return View();

            if (!service.Photo.CheckFileType("image/"))
            {
                ModelState.AddModelError("Photo", "Please choose correct image type");
                return View();
            }

            if (!service.Photo.CheckFileSize(200000))
            {
                ModelState.AddModelError("Photo", "Please choose correct image size");
                return View();
            }

            string fileName = Guid.NewGuid().ToString() + "_" + service.Photo.FileName;

            string path = Helper.GetFilePath(_env.WebRootPath, "assets/img/features", fileName);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await service.Photo.CopyToAsync(stream);
            }

            Service newService = new Service
            {
                Title = service.Title,
                Image = fileName,
            };

            await _context.Services.AddAsync(newService);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int id)
        {
            Service services = await GetByIdAsync(id);

            if (services == null) return NotFound();

            string path = Helper.GetFilePath(_env.WebRootPath, "img", services.Image);


            Helper.DeleteFile(path);

            _context.Services.Remove(services);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Services
        private async Task<Service> GetByIdAsync(int id)
        {
            return await _context.Services.FindAsync(id);
        }

        private List<ServiceListVM> GetMapDatas(List<Service> services)
        {
            List<ServiceListVM> serviceListVMs = new List<ServiceListVM>();

            foreach (var item in services)
            {
                ServiceListVM newService = new ServiceListVM
                {
                    Id = item.Id,
                    Title = item.Title,
                    Image = item.Image
                };

                serviceListVMs.Add(newService);
            }

            return serviceListVMs;
        }

        private async Task<int> GetPageCount(int take)
        {
            int serviceCount = await _context.Services.Where(m => !m.IsDeleted).CountAsync();

            return (int)Math.Ceiling((decimal)serviceCount / take);
        }

        #endregion
    }
}
