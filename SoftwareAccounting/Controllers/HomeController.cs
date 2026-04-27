using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SoftwareAccounting.Models;
namespace SoftwareAccounting.Controllers;
/*Стандартный контроллер для домашней страницы, страницы конфиденциальности и ошибок.

Index – главная страница (пустая заглушка).

Privacy – страница политики конфиденциальности.

Error – страница ошибок.*/
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }
    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
