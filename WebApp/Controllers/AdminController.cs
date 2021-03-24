// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace WebApp.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("LogInAdmin", "Account");
        }
    }
}