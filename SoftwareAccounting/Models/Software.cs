using System.ComponentModel.DataAnnotations;
namespace SoftwareAccounting.Models
/*Модель ПО из справочника:

Id

Name – название

License – тип лицензии (из перечисления)

SoftwareType – категория (OS, Office, Development и т.п.)*/
{
    public class Software
    {
        public int Id { get; set; }
        [Display(Name = "Название ПО")]
        public string Name { get; set; } = string.Empty;
        [Display(Name = "Тип лицензии")]
        public LicenseType License { get; set; }
        [Display(Name = "Тип ПО")]
        public string SoftwareType { get; set; } = "General";
    }
}