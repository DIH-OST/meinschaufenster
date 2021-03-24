// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using WebApp.Services;

namespace WebApp.Controllers
{
    [Authorize(Roles = "User")]
    public class DemoUserController : Controller
    {
        readonly UserManager _userManager;

        public DemoUserController(UserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            string benutzerRolle = User.FindFirst(ClaimTypes.Role).Value;
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            string companyName = User.FindFirst(ClaimTypes.Name).Value;

            var name = CultureInfo.CurrentUICulture.Name;
            var name2 = CultureInfo.CurrentCulture.Name;

            //ViewBag.Text = $"Rolle: {benutzerRolle}, ID: {userId}, Firmenname: {companyName}";
            ViewBag.Text = $"Rolle: {name}, ID: {name2}";

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            _userManager.SignOut(HttpContext);
            return RedirectToAction("Index", "Home");
        }
    }
}