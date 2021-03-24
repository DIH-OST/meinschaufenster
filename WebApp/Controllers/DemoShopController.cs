// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Shop")]
    public class DemoShopController : Controller
    {
        public IActionResult Index()
        {
            string benutzerRolle = User.FindFirst(ClaimTypes.Role).Value;
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            string companyName = User.FindFirst(ClaimTypes.Name).Value;

            ViewBag.Text = $"Rolle: {benutzerRolle}, ID: {userId}, Firmenname: {companyName}";

            return View();
        }
    }
}