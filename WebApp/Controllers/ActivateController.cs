// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;

namespace WebApp.Controllers
{
    public class ActivateController : Controller
    {
        private readonly Db _context;

        /// <summary>
        ///     Konto
        /// </summary>
        /// <param name="userManager"></param>
        public ActivateController(Db context)
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

                TempData["message"] = "Du kannst dich jetzt mit dem zugesandten Passwort einloggen.";

                if (!e.Activated)
                {
                    TempData["message"] = "Willkommen! " + TempData["message"];
                }

                e.Activated = true;
                await db.SaveChangesAsync();


                return RedirectToAction("LogInShop", "Account");
            }
        }
    }
}