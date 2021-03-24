// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;
using Exchange;
using Exchange.Model;
using Exchange.ServiceAccess;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly Db _context;
        readonly UserManager _userManager;

        /// <summary>
        ///     Konto
        /// </summary>
        /// <param name="userManager"></param>
        public AccountController(UserManager userManager, Db context)
        {
            _userManager = userManager;
            _context = context;
        }

        /// <summary>
        ///     Login
        /// </summary>
        /// <returns></returns>
        public IActionResult LoginUser(string returnUrl = "")
        {
            if (TempData["message"] != null) ViewData["message"] = TempData["message"];

            ViewData["ReturnUrl"] = returnUrl;

            return View("LoginUser");
        }

        /// <summary>
        ///     Login
        /// </summary>
        /// <returns></returns>
        public IActionResult LoginShop()
        {
            if (TempData["message"] != null) ViewData["message"] = TempData["message"];
            return View("LoginShop");
        }


        /// <summary>
        ///     Login
        /// </summary>
        /// <returns></returns>
        public IActionResult LoginAdmin()
        {
            if (TempData["message"] != null) ViewData["message"] = TempData["message"];
            return View("LoginAdmin");
        }

        /// <summary>
        ///     Registrierung Shop
        /// </summary>
        /// <returns></returns>
        public IActionResult RegisterShop()
        {
            if (TempData["message"] != null) ViewData["message"] = TempData["message"];
            return View("RegisterShop");
        }

        /// <summary>
        ///     Registrierung Endkunde
        /// </summary>
        /// <returns></returns>
        public IActionResult RegisterUser()
        {
            if (TempData["message"] != null) ViewData["message"] = TempData["message"];
            return View("RegisterUser");
        }

        /// <summary>
        ///     Registierung Shop
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterUserViewModel form)
        {
            if (!ModelState.IsValid)
                return View(form);
            try
            {
                var success = ValidationHelper.ProoveValidPhoneNumber(form.PhoneNumber, out string telNumber);
                if (!success)
                {
                    ModelState.AddModelError("PhoneNumber", "Die Telefonnummer hat ein ungültiges Format");
                    return View(form);
                }

                var user = await _context.TblUsers.FirstOrDefaultAsync(a => a.PhoneNumber == telNumber);

                if (user != null)
                {
                    //ModelState.AddModelError("PhoneNumber", "Diese Telefonnumer existiert bereits im System.");
                    //return View(form);
                    TempData["message"] = "Du bist bereits registriert. Bitte logge dich ein!";
                    return RedirectToAction("LoginUser");
                }

                RestAccess ra = new RestAccess(Constants.ServiceClientEndPointWithApiPrefix);
                var res = await ra.UserCheck(telNumber);

                if (res.Ok)
                {
                    if (!res.Result.IsNewUser)
                    {
                        TempData["message"] = "Du bist bereits registriert. Bitte logge dich ein!";
                        return RedirectToAction("LoginUser");
                    }

                    TempData["message"] = "Danke! Um die Registrierung abzuschließen überprüfe bitte deine SMS Nachrichten!";
                    return RedirectToAction("LoginUser");
                }

                ViewData["message"] = "Registrierung war nicht erfolgreich. Bitte Eingabe überprüfen!";
                return View(form);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("PhoneNumber", ex.Message);
                return View(form);
            }
        }

        public async Task<IActionResult> ForgotPasswordUser()
        {
            return View(new RegisterUserViewModel());
        }

        /// <summary>
        ///     sendet das Passwort erneut zu
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ForgotPasswordUser(RegisterUserViewModel form)
        {
            if (!ModelState.IsValid)
                return View(form);
            try
            {
                var success = ValidationHelper.ProoveValidPhoneNumber(form.PhoneNumber, out string telNumber);
                if (!success)
                {
                    ModelState.AddModelError("PhoneNumber", "Die Telefonnummer hat ein ungültiges Format");
                    return View(form);
                }

                var user = await _context.TblUsers.FirstOrDefaultAsync(a => a.PhoneNumber == telNumber);

                if (user == null)
                {
                    ModelState.AddModelError("PhoneNumber", "Diese Telefonummer existiert nicht!");
                    return View(form);
                }

                RestAccess ra = new RestAccess(Constants.ServiceClientEndPointWithApiPrefix);
                var res = await ra.UserStartResetPassword(user.Id);

                if (res.Ok)
                {
                    TempData.Add("message", "Überprüfe bitte deine SMS Nachrichten!");
                    return RedirectToAction("LoginUser");
                }

                ViewData["message"] = "Passwort konnte nicht verschickt werden!";
                return View(form);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("PhoneNumber", ex.Message);
                return View(form);
            }
        }

        /// <summary>
        ///     Logout
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            _userManager.SignOut(HttpContext);
            return RedirectToAction("Index", "Home");
        }


        /// <summary>
        ///     Login Post
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LogInUser(LoginViewModel form, string returnUrl = "")
        {
            if (!ModelState.IsValid)
                return View(form);
            try
            {
                var numberOk = ValidationHelper.ProoveValidPhoneNumber(form.UserName, out var num);

                if (!numberOk)
                {
                    ModelState.AddModelError("Username", "Bitte überprüfe die Eingabe der Telefonnummer.");
                    return View(form);
                }

                var eUser = await _context.TblUsers.FirstOrDefaultAsync(a => a.PhoneNumber == num);

                if (eUser == null)
                {
                    var ls = await _userManager.SignInUser(HttpContext, num, form.Password?.Trim());

                    ViewData["message"] = "Überprüfe bitte deine SMS Nachrichten! Wir haben dir ein Passwort zugesandt mit dem du dich gleich einloggen kannst.";
                    form.Password = "";

                    return View("LoginUser", form);
                }

                var loginSuccess = await _userManager.SignInUser(HttpContext, num, form.Password?.Trim());

                if (loginSuccess)
                {
                    if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index", "MyAppointments", null);
                }

                ModelState.AddModelError("Password", "Benutzer konnte nicht angemeldet werden. Bitte überprüfen deinen Benutzernamen und dein Passwort.");
                return View(form);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Password", ex.Message);
                return View(form);
            }
        }

        /// <summary>
        ///     Login Post
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LogInShop(LoginViewModel form)
        {
            if (!ModelState.IsValid)
                return View(form);
            try
            {
                var loginSuccess = 1;
                if (!(form.UserName is null))
                {
                    form.UserName = form.UserName.Trim().Replace(" ", "").ToLower();
                    loginSuccess = await _userManager.SignInShop(HttpContext, form.UserName, form.Password);
                }

                if (loginSuccess == 0)
                    return RedirectToAction("StoreData", "Stores", null);
                if (loginSuccess == 1)
                    ModelState.AddModelError("Password", "Benutzer konnte nicht angemeldet werden. Bitte überprüfen Sie Benutzername und Passwort.");
                else
                {
                    ModelState.AddModelError("Password", "Benutzer wurde noch nicht bestätigt. Bitte E-Mail Ordner überprüfen oder erneut Registrieren um ein neues Bestätigungs E-Mail zugesandt zu bekommen.");
                }

                return View(form);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Password", ex.Message);
                return View(form);
            }
        }

        public async Task<IActionResult> ForgotPasswordShop()
        {
            return View(new RegisterShopViewModel());
        }

        /// <summary>
        ///     Registierung Shop
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ForgotPasswordShop(RegisterShopViewModel form)
        {
            if (!ModelState.IsValid)
                return View(form);

            try
            {
                if (String.IsNullOrEmpty(form.UserName))
                {
                    ModelState.AddModelError("Username", "Bitte eine E-Mail Adresse angeben.");
                    return View(form);
                }

                var email = form.UserName?.Trim().Replace(" ", "").ToLower();

                RestAccess ra = new RestAccess(Constants.ServiceClientEndPointWithApiPrefix);
                await ra.ForgotPasswordShop(new ExShopForgotPassword {EMail = email, Step = EnumShopForgotPassword.Step1});

                var store = await _context.TblStores.FirstOrDefaultAsync(a => a.EMail.ToLower() == email && a.Activated);

                if (store == null)
                {
                    ModelState.AddModelError("Username", "Diese E-Mail existiert nicht im System.");
                    return View(form);
                }

                if (!store.Activated)
                {
                    await ra.RegisterShop(new ExShopRegistration {EMail = email});
                }

                TempData["message"] = "Bitte überprüfe deinen Posteingang. Dir wurde ein Bestätigungs-Link zugesandt.";

                return RedirectToAction("LogInShop");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Username", "Unbekannter Fehler. Bitte versuche es in wenigen Minuten erneut.");
                return View(form);
            }
        }

        /// <summary>
        ///     Registierung Shop
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RegisterShop(RegisterShopViewModel form)
        {
            if (!ModelState.IsValid)
                return View(form);
            try
            {
                if (String.IsNullOrEmpty(form.UserName))
                {
                    ModelState.AddModelError("Username", "Bitte eine E-Mail Adresse angeben.");
                    return View(form);
                }

                var email = form.UserName.Trim().Replace(" ", "").ToLower();
                var store = await _context.TblStores.FirstOrDefaultAsync(a => a.EMail.ToLower() == email && a.Activated);

                if (store != null)
                {
                    ModelState.AddModelError("Username", "Diese E-Mail existiert bereits im System.");
                    return View(form);
                }

                RestAccess ra = new RestAccess(Constants.ServiceClientEndPointWithApiPrefix);
                await ra.RegisterShop(new ExShopRegistration {EMail = email});

                ViewBag.Message = "Danke! Um die Registrierung abzuschließen überprüfe bitte deinen Posteingang!";
                return View("Message");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Password", ex.Message);
                return View(form);
            }
        }

        public async Task<IActionResult> LoginAdmin2Test()
        {
            //var loginSuccess = await _userManager.SignInAdmin(HttpContext, "admin", "!mein!schau!fenster");
            //return RedirectToAction("Index", "Stores", null);
            return RedirectToAction("LoginAdmin", null);
        }

        /// <summary>
        ///     Login Post
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LogInAdmin(LoginViewModel form)
        {
            if (!ModelState.IsValid)
                return View(form);
            try
            {
                var loginSuccess = await _userManager.SignInAdmin(HttpContext, "admin", form.Password);

                if (loginSuccess)
                    return RedirectToAction("Index", "ProductCategories", null);
                ModelState.AddModelError("Password", "Benutzer konnte nicht angemeldet werden. Bitte überprüfen Sie das Passwort.");
                return View(form);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Password", ex.Message);
                return View(form);
            }
        }
    }
}