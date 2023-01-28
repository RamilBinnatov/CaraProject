using FinalProject.Data;
using FinalProject.Models;
using FinalProject.Services;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Controllers
{
    public class ContactController : Controller
    {
        #region Readonly
        private readonly AppDbContext _context;
        private readonly LayoutService _layoutService;
        #endregion

        #region Constructor
        public ContactController(AppDbContext context, LayoutService layoutService)
        {
            _context = context;
            _layoutService = layoutService;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index()
        {
            Dictionary<string, string> settingDatas = await _layoutService.GetDatasFromSetting();
            string Address = settingDatas["Address"];
            string PhoneNumber = settingDatas["PhoneNumber"];
            string WorkingHours = settingDatas["WorkingHours"];
            string Email = settingDatas["Email"];
            IEnumerable<Employee> employees = await _context.Employees.Where(m => !m.IsDeleted).Take(3).ToListAsync();

            ViewBag.Address = Address;
            ViewBag.PhoneNumber = PhoneNumber;
            ViewBag.WorkingHours = WorkingHours;
            ViewBag.Email = Email;

            //ContactVM contactVM = new ContactVM
            //{
            //    employees = employees
            //};
            return View(employees);
        }
        #endregion

        #region ContactUs
        [HttpPost]
        public async Task<IActionResult> ContactUs(Contact contact)
        {
            await _context.Contacts.AddAsync(contact);
            await _context.SaveChangesAsync();
            return RedirectToAction("index");
        }
        #endregion
    }
}
