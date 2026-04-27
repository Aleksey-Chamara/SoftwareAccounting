using Microsoft.AspNetCore.Identity;
using SoftwareAccounting.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
namespace SoftwareAccounting.Data
/*Инициализатор базы данных, вызывается при запуске.

Создаёт роли Admin и User, если их нет.

Создаёт пользователей: admin (пароль admin, роль Admin) и user (пароль user, роль User).

Если таблица Softwares пуста, добавляет 5 демо-программ (Windows, Office, WinRAR, VS Code, Kaspersky).

Создаёт три компьютера (директор, бухгалтер, разработчик) и устанавливает на них ПО (включая кастомное).*/
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            if (await userManager.FindByNameAsync("admin") == null)
            {
                var adminUser = new IdentityUser { UserName = "admin", Email = "admin@local.test", EmailConfirmed = true };
                var result = await userManager.CreateAsync(adminUser, "admin");
                if (result.Succeeded) await userManager.AddToRoleAsync(adminUser, "Admin");
            }
            if (await userManager.FindByNameAsync("user") == null)
            {
                var simpleUser = new IdentityUser { UserName = "user", Email = "user@local.test", EmailConfirmed = true };
                var result = await userManager.CreateAsync(simpleUser, "user");
                if (result.Succeeded) await userManager.AddToRoleAsync(simpleUser, "User");
            }
            if (!context.Softwares.Any())
            {
                var softWindows = new Software { Name = "Windows 11 Pro", License = LicenseType.Licensed, SoftwareType = "OS" };
                var softOffice = new Software { Name = "MS Office 2021", License = LicenseType.Licensed, SoftwareType = "Office" };
                var softWinRar = new Software { Name = "WinRAR", License = LicenseType.Shareware, SoftwareType = "Archiver" };
                var softVsCode = new Software { Name = "VS Code", License = LicenseType.Free, SoftwareType = "Development" };
                var softKaspersky = new Software { Name = "Kaspersky Endpoint", License = LicenseType.Trial, SoftwareType = "Antivirus" };
                context.Softwares.AddRange(softWindows, softOffice, softWinRar, softVsCode, softKaspersky);
                await context.SaveChangesAsync();
                var pcDirector = new Computer 
                { 
                    Owner = "Иванов И.И. (Директор)", 
                    MacAddress = "00-1B-44-11-3A-B7", 
                    LastCheckDate = DateTime.Now.AddDays(-5) 
                };
                var pcAccountant = new Computer 
                { 
                    Owner = "Петрова А.С. (Бухгалтер)", 
                    MacAddress = "A1-B2-C3-D4-E5-F6", 
                    LastCheckDate = DateTime.Now.AddDays(-20) 
                };
                var pcDeveloper = new Computer 
                { 
                    Owner = "Сидоров В.К. (Разработчик)", 
                    MacAddress = "AA-BB-CC-11-22-33", 
                    LastCheckDate = DateTime.Now 
                };
                context.Computers.AddRange(pcDirector, pcAccountant, pcDeveloper);
                await context.SaveChangesAsync();
                context.InstalledSoftwares.Add(new InstalledSoftware { ComputerId = pcDirector.Id, SoftwareId = softWindows.Id, InstallDate = DateTime.Now.AddMonths(-12) });
                context.InstalledSoftwares.Add(new InstalledSoftware { ComputerId = pcDirector.Id, SoftwareId = softOffice.Id, InstallDate = DateTime.Now.AddMonths(-12) });
                context.InstalledSoftwares.Add(new InstalledSoftware { ComputerId = pcAccountant.Id, SoftwareId = softWindows.Id, InstallDate = DateTime.Now.AddMonths(-6) });
                context.InstalledSoftwares.Add(new InstalledSoftware { ComputerId = pcAccountant.Id, SoftwareId = softOffice.Id, InstallDate = DateTime.Now.AddMonths(-6) });
                context.InstalledSoftwares.Add(new InstalledSoftware { ComputerId = pcAccountant.Id, CustomSoftwareName = "1C: Бухгалтерия 8.3", InstallDate = DateTime.Now.AddMonths(-5) }); 
                context.InstalledSoftwares.Add(new InstalledSoftware { ComputerId = pcDeveloper.Id, SoftwareId = softWindows.Id, InstallDate = DateTime.Now.AddMonths(-2) });
                context.InstalledSoftwares.Add(new InstalledSoftware { ComputerId = pcDeveloper.Id, SoftwareId = softVsCode.Id, InstallDate = DateTime.Now.AddMonths(-2) });
                context.InstalledSoftwares.Add(new InstalledSoftware { ComputerId = pcDeveloper.Id, SoftwareId = softWinRar.Id, InstallDate = DateTime.Now.AddMonths(-2) });
                context.InstalledSoftwares.Add(new InstalledSoftware { ComputerId = pcDeveloper.Id, CustomSoftwareName = "Docker Desktop", InstallDate = DateTime.Now.AddDays(-10) }); 
                await context.SaveChangesAsync();
            }
        }
    }
}