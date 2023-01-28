using FinalProject.Data;
using FinalProject.Helpers.Enums;
using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
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
    public class ContactController : Controller
    {
        #region readonly
        private readonly AppDbContext _context;
        #endregion

        #region Constructor
        public ContactController(AppDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index()
        {
            IEnumerable<Contact> contacts = await _context.Contacts.Where(m => !m.IsDeleted).ToListAsync();
            return View(contacts);
        }
        #endregion

        #region Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            Contact contact = await _context.Contacts.FindAsync(id);

            if (contact == null) return NotFound();

            _context.Contacts.Remove(contact);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Detail
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Contact contact = await _context.Contacts.FindAsync(id);

            if (contact == null) return NotFound();

            return View(contact);
        }
        #endregion
    }
}
