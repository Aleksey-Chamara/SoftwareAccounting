using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace SoftwareAccounting.Controllers
/*Управляет аутентификацией.

GET Login – показывает страницу входа.

POST Login – проверяет логин/пароль через SignInManager, при успехе перенаправляет на Computers / Index.

Logout – выполняет выход и перенаправляет на страницу входа.*/
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        public AccountController(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Login() => View();
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Computers");
            }
            ViewBag.Error = "Неверный логин или пароль";
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}