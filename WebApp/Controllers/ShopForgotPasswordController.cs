// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;
using Exchange;
using Exchange.Model;
using Exchange.ServiceAccess;

namespace WebApp.Controllers
{
    public class ShopForgotPasswordController : Controller
    {
        private readonly Db _context;

        /// <summary>
        ///     Konto
        /// </summary>
        /// <param name="userManager"></param>
        public ShopForgotPasswordController(Db context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string guid)
        {
            using (Db db = new Db())
            {
                var e = db.TblStores.FirstOrDefault(a => a.ActivationCode == guid);
                if (e == null)
                {
                    return RedirectToAction("RegisterShop", "Account");
                }

                RestAccess ra = new RestAccess(Constants.ServiceClientEndPointWithApiPrefix);
                await ra.ForgotPasswordShop(new ExShopForgotPassword {EMail = e.EMail, Step = EnumShopForgotPassword.Step2});
            }

            TempData["message"] = "Bitte überprüfe deinen Posteingang! Dir wurde ein neues Passwort zugesandt!";

            return RedirectToAction("LogInShop", "Account");
        }
    }
}