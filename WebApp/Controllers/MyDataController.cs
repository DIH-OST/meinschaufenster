// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;
using Database.Tables;
using Exchange;
using Exchange.PostRequests;
using Exchange.ServiceAccess;
using WebApp.Services;
using WebRestApi.Helper;

namespace WebApp.Controllers
{
    [Authorize(Roles = "User")]
    public class MyDataController : Controller
    {
        private readonly Db _context;
        private readonly UserManager _userManager;

        public MyDataController(UserManager userManager, Db context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (TempData["message"] != null) ViewData["message"] = TempData["message"];
            if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
            {
                var tableUser = await _context.TblUsers.FindAsync(userId);
                if (tableUser == null)
                {
                    return NotFound();
                }

                return View(tableUser);
            }

            return View();
        }

        // POST: MyData/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Locked,PhoneNumber,PhoneChecked,DefaultUserLanguage,Password,RestPasswort,CreatedAtUtc,IsDemoUser,IsAdmin,Firstname,Lastname,Street,PostalCode,City")]
            TableUser tableUser)
        {
            if (id != tableUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tableUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TableUserExists(tableUser.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(tableUser);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser()
        {
            //user löschen
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var tbUser = await _context.TblUsers.AsNoTracking().FirstOrDefaultAsync(a => a.Id == userId);
            RestAccess ra = new RestAccess(tbUser.Id.ToString(), tbUser.RestPasswort, Constants.ServiceClientEndPointWithApiPrefix);

            var res = await ra.DeleteUserWeb(new ExDeleteRequest
                                             {
                                                 Id = userId,
                                                 CheckPassword = WebAppSettings.CheckPassword,
                                             });

            if (res.Ok)
            {
                if (res.Result.Result == EnumSaveDataResult.Error)
                {
                    TempData["message"] = res.Result.Description;
                    return RedirectToAction("Index");
                }

                //user ausloggen
                return RedirectToAction("Logout", "Account");
            }

            TempData["message"] = "Benutzer konnte nicht gelöscht werden";
            return RedirectToAction("Index");
        }

        private bool TableUserExists(int id)
        {
            return _context.TblUsers.Any(e => e.Id == id);
        }
    }
}