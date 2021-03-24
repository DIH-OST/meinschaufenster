// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;
using Database.Tables;
using WebApp.Services;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly Db _context;
        private readonly UserManager _userManager;

        public UsersController(Db context, UserManager userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Users
        // A simple View is returned
        // The Grid will asyc call the Users_Read action on page load
        public IActionResult Index() => View();

        public async Task<IActionResult> TblUsers_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(await _context.TblUsers.ToDataSourceResultAsync(request));
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> SignInAsUser(int userId)
        {
            await _userManager.SignInUserForAdmin(HttpContext, userId);
            return RedirectToAction("Index", "MyAppointments");
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Locked,PhoneNumber,PhoneChecked,DefaultUserLanguage,Password,RestPasswort,CreatedAtUtc,IsDemoUser,IsAdmin,Firstname,Lastname,Street,PostalCode,City")]
            TableUser tableUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tableUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(tableUser);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tableUser = await _context.TblUsers.FindAsync(id);
            if (tableUser == null)
            {
                return NotFound();
            }

            return View(tableUser);
        }

        // POST: Users/Edit/5
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

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tableUser = await _context.TblUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tableUser == null)
            {
                return NotFound();
            }

            return View(tableUser);
        }

        // POST: Users/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tableUser = await _context.TblUsers.FindAsync(id);
            _context.TblUsers.Remove(tableUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TableUserExists(int id)
        {
            return _context.TblUsers.Any(e => e.Id == id);
        }
    }
}