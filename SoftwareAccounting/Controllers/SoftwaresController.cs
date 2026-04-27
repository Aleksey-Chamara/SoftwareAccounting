using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftwareAccounting.Data;
using SoftwareAccounting.Models;
namespace SoftwareAccounting.Controllers
/*Управляет справочником программного обеспечения.

Index – список всех программ с сортировкой по названию, лицензии, типу.

Create – добавление новой программы (только для администратора).*/
{
    [Authorize]
    public class SoftwaresController : Controller
    {
        private readonly AppDbContext _context;
        public SoftwaresController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["NameSort"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["LicenseSort"] = sortOrder == "License" ? "license_desc" : "License";
            ViewData["TypeSort"] = sortOrder == "Type" ? "type_desc" : "Type";
            var softwares = from s in _context.Softwares
                            select s;
            switch (sortOrder)
            {
                case "name_desc":
                    softwares = softwares.OrderByDescending(s => s.Name);
                    break;
                case "License":
                    softwares = softwares.OrderBy(s => s.License);
                    break;
                case "license_desc":
                    softwares = softwares.OrderByDescending(s => s.License);
                    break;
                case "Type":
                    softwares = softwares.OrderBy(s => s.SoftwareType);
                    break;
                case "type_desc":
                    softwares = softwares.OrderByDescending(s => s.SoftwareType);
                    break;
                default:
                    softwares = softwares.OrderBy(s => s.Name);
                    break;
            }
            return View(await softwares.AsNoTracking().ToListAsync());
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,License,SoftwareType")] Software software)
        {
            if (ModelState.IsValid)
            {
                _context.Add(software);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(software);
        }
    }
}