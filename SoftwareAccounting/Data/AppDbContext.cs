using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SoftwareAccounting.Models;
namespace SoftwareAccounting.Data
/*Контекст базы данных, наследуется от IdentityDbContext.
Содержит три DbSet:

Computers

Softwares

InstalledSoftwares
Обеспечивает связь с SQLite через Entity Framework Core.*/
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<Computer> Computers { get; set; }
        public DbSet<Software> Softwares { get; set; }
        public DbSet<InstalledSoftware> InstalledSoftwares { get; set; }
    }
}