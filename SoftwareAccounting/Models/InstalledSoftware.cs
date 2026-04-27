using System.ComponentModel.DataAnnotations;
namespace SoftwareAccounting.Models
/*Модель связи «компьютер – ПО» (таблица многие-ко-многим):

Id

ComputerId (внешний ключ)

SoftwareId (может быть null – для ручного ввода)

CustomSoftwareName – название, если ПО не из справочника

InstallDate – дата установки

IsOther – вычисляемое свойство (true, если SoftwareId == null).*/
{
    public class InstalledSoftware
    {
        public int Id { get; set; }
        public int ComputerId { get; set; }
        public Computer? Computer { get; set; }
        public int? SoftwareId { get; set; }
        public Software? Software { get; set; }
        [Display(Name = "Название (если Other)")]
        public string? CustomSoftwareName { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Дата установки")]
        public DateTime InstallDate { get; set; }
        public bool IsOther => SoftwareId == null;
    }
}