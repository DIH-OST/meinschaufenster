// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace WebApp.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("LoginAdmin", "Account");

            //return RedirectToAction("LoginAdmin2Test", "Account");
        }
    }
}