namespace SoftwareAccounting.Models;
/*Модель для страницы ошибок. Содержит RequestId и флаг ShowRequestId.*/
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
