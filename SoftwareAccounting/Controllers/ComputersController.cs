using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoftwareAccounting.Data;
using SoftwareAccounting.Models;
namespace SoftwareAccounting.Controllers
/*Основной контроллер для работы с компьютерами и установленным ПО.

Index – список компьютеров с сортировкой (по владельцу, MAC, дате) и поиском по владельцу.

Details – паспорт компьютера с таблицей установленного ПО (сортировка по названию, лицензии, типу, дате).

Create, Edit, Delete – CRUD-операции (только для администратора).

ManageSoftware – страница управления ПО для конкретного компьютера (админ).

AddSoftwareFromList – добавляет ПО из справочника.

AddCustomSoftware – добавляет произвольное ПО (ручной ввод).

RemoveSoftware – удаляет запись об установке.*/
{
    [Authorize]
    public class ComputersController : Controller
    {
        private readonly AppDbContext _context;
        public ComputersController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string sortOrder, string searchString, string softwareSearch)
        {
            ViewData["OwnerSort"] = String.IsNullOrEmpty(sortOrder) ? "owner_desc" : "";
            ViewData["MacSort"] = sortOrder == "Mac" ? "mac_desc" : "Mac";
            ViewData["DateSort"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["CurrentFilter"] = searchString;
            ViewData["SoftwareFilter"] = softwareSearch; // ← новое

            var computers = _context.Computers
                .Include(c => c.Installations)
                    .ThenInclude(i => i.Software)
                .AsQueryable();

            // Фильтр по владельцу (существующий)
            if (!String.IsNullOrEmpty(searchString))
            {
                computers = computers.Where(c => c.Owner.Contains(searchString));
            }

            // ← Фильтр по названию ПО (новый)
            if (!String.IsNullOrEmpty(softwareSearch))
            {
                computers = computers.Where(c => c.Installations.Any(i =>
                    (i.Software != null && i.Software.Name.Contains(softwareSearch)) ||
                    (i.CustomSoftwareName != null && i.CustomSoftwareName.Contains(softwareSearch))
                ));
            }

            switch (sortOrder)
            {
                case "owner_desc":
                    computers = computers.OrderByDescending(s => s.Owner);
                    break;
                case "Mac":
                    computers = computers.OrderBy(s => s.MacAddress);
                    break;
                case "mac_desc":
                    computers = computers.OrderByDescending(s => s.MacAddress);
                    break;
                case "Date":
                    computers = computers.OrderBy(s => s.LastCheckDate);
                    break;
                case "date_desc":
                    computers = computers.OrderByDescending(s => s.LastCheckDate);
                    break;
                default:
                    computers = computers.OrderBy(s => s.Owner);
                    break;
            }

            return View(await computers.AsNoTracking().ToListAsync());
        }
        public async Task<IActionResult> Details(int? id, string sortOrder)
        {
            if (id == null) return NotFound();
            var computer = await _context.Computers
                .Include(c => c.Installations)
                .ThenInclude(i => i.Software)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (computer == null) return NotFound();
            ViewData["NameSort"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["LicenseSort"] = sortOrder == "License" ? "license_desc" : "License";
            ViewData["TypeSort"] = sortOrder == "Type" ? "type_desc" : "Type";
            ViewData["DateSort"] = sortOrder == "Date" ? "date_desc" : "Date";
            if (computer.Installations != null && computer.Installations.Any())
            {
                IEnumerable<InstalledSoftware> sortedList = computer.Installations;
                switch (sortOrder)
                {
                    case "name_desc":
                        sortedList = sortedList.OrderByDescending(i => i.IsOther ? i.CustomSoftwareName : i.Software?.Name);
                        break;
                    case "License":
                        sortedList = sortedList.OrderBy(i => i.Software?.License);
                        break;
                    case "license_desc":
                        sortedList = sortedList.OrderByDescending(i => i.Software?.License);
                        break;
                    case "Type":
                        sortedList = sortedList.OrderBy(i => i.Software?.SoftwareType ?? "Прочее");
                        break;
                    case "type_desc":
                        sortedList = sortedList.OrderByDescending(i => i.Software?.SoftwareType ?? "Прочее");
                        break;
                    case "Date":
                        sortedList = sortedList.OrderBy(i => i.InstallDate);
                        break;
                    case "date_desc":
                        sortedList = sortedList.OrderByDescending(i => i.InstallDate);
                        break;
                    default:
                        sortedList = sortedList.OrderBy(i => i.IsOther ? i.CustomSoftwareName : i.Software?.Name);
                        break;
                }
                computer.Installations = sortedList.ToList();
            }
            return View(computer);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Owner,MacAddress,LastCheckDate")] Computer computer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(computer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(computer);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var computer = await _context.Computers.FindAsync(id);
            if (computer == null) return NotFound();
            return View(computer);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Owner,MacAddress,LastCheckDate")] Computer computer)
        {
            if (id != computer.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(computer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComputerExists(computer.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(computer);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var computer = await _context.Computers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (computer == null) return NotFound();
            return View(computer);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var computer = await _context.Computers.FindAsync(id);
            if (computer != null)
            {
                _context.Computers.Remove(computer);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageSoftware(int? id)
        {
            if (id == null) return NotFound();
            var computer = await _context.Computers
                .Include(c => c.Installations)
                .ThenInclude(i => i.Software)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (computer == null) return NotFound();
            ViewData["SoftwareList"] = new SelectList(_context.Softwares, "Id", "Name");
            return View(computer);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddSoftwareFromList(int computerId, int softwareId, DateTime installDate)
        {
            var install = new InstalledSoftware
            {
                ComputerId = computerId,
                SoftwareId = softwareId,
                InstallDate = installDate
            };
            _context.InstalledSoftwares.Add(install);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageSoftware), new { id = computerId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCustomSoftware(int computerId, string customName, DateTime installDate)
        {
            if (!string.IsNullOrWhiteSpace(customName))
            {
                var install = new InstalledSoftware
                {
                    ComputerId = computerId,
                    SoftwareId = null, // null = Other
                    CustomSoftwareName = customName,
                    InstallDate = installDate
                };
                _context.InstalledSoftwares.Add(install);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ManageSoftware), new { id = computerId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveSoftware(int id)
        {
            var install = await _context.InstalledSoftwares.FindAsync(id);
            if (install != null)
            {
                int computerId = install.ComputerId;
                _context.InstalledSoftwares.Remove(install);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageSoftware), new { id = computerId });
            }
            return RedirectToAction(nameof(Index));
        }
        private bool ComputerExists(int id)
        {
            return _context.Computers.Any(e => e.Id == id);
        }
    }
}