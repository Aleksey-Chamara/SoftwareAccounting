using System.ComponentModel.DataAnnotations;
namespace SoftwareAccounting.Models
/*Модель компьютера:

Id – первичный ключ.

Owner – владелец (строка).

MacAddress – MAC-адрес.

LastCheckDate – дата последней проверки.

Installations – навигационное свойство (список установленного ПО).*/
{
    public class Computer
    {
        public int Id { get; set; }
        [Display(Name = "Владелец")]
        public string Owner { get; set; } = string.Empty;
        [Display(Name = "MAC адрес")]
        public string MacAddress { get; set; } = string.Empty;
        [DataType(DataType.Date)]
        [Display(Name = "Дата последней проверки")]
        public DateTime LastCheckDate { get; set; }
        public List<InstalledSoftware> Installations { get; set; } = new();
    }
}