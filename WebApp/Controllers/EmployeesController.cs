// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;
using Database.Tables;
using WebRestApi.Helper;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Shop")]
    public class EmployeesController : Controller
    {
        private readonly Db _context;

        public EmployeesController(Db context)
        {
            _context = context;
        }

        // GET: Employees
        // A simple View is returned
        // The Grid will asyc call the Employees_Read action on page load
        public IActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewData["message"] = TempData["message"];
            }

            return View();
        }

        public async Task<IActionResult> TblEmployees_Read([DataSourceRequest] DataSourceRequest request)
        {
            var storeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return Json(await _context.TblEmployees.Where(x => x.StoreId == storeId).ToDataSourceResultAsync(request));
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            var emp = new TableEmployee
                      {
                          StoreId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)
                      };

            return View(emp);
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,TelephoneNumber,DefaultAnnotation,Active,Image,StoreId")]
            TableEmployee tableEmployee, IFormFile picture)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (tableEmployee.StoreId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                    {
                        return Forbid();
                    }

                    if (_context.TblEmployees.Count(a => a.StoreId == tableEmployee.StoreId) >= 1)
                    {
                        TempData["message"] = "Zur Zeit kann nur ein Mitarbeiter angelegt werden!";
                        return RedirectToAction("Employees", "Stores");
                    }

                    tableEmployee.Image = CreateImage(picture);

                    var storeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                    //Zuweisung zu SecondaryLocation!
                    var sec = await _context.TblLocations.FirstOrDefaultAsync(a => a.StoreId == storeId);

                    sec.TblLocationEmployee = new List<TableLocationEmployee>();
                    sec.TblLocationEmployee.Add(new TableLocationEmployee
                                                {
                                                    TblEmployee = tableEmployee
                                                });
                    //tableEmployee.TblSecondaryLocation = new List<TableSecondaryLocation>();
                    //tableEmployee.TblSecondaryLocation.Add(sec);

                    if (!String.IsNullOrEmpty(tableEmployee.TelephoneNumber))
                    {
                        string outPhoneNumber;
                        var a = ValidationHelper.ProoveValidPhoneNumber(tableEmployee.TelephoneNumber, out outPhoneNumber);
                        //TODO: Meldung wenn nicht passt
                        tableEmployee.TelephoneNumber = outPhoneNumber;
                    }

                    _context.Add(tableEmployee);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "Daten erfolgreich gespeichert";
                    return RedirectToAction("Employees", "Stores");
                }
                catch (Exception)
                {
                    ViewData["message"] = "Daten konnten nicht gespeichert werden!";
                }
            }

            return View(tableEmployee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (TempData.ContainsKey("message"))
            {
                ViewData["message"] = TempData["message"];
            }

            if (id == null)
            {
                return NotFound();
            }

            var tableEmployee = await _context.TblEmployees.FindAsync(id);
            if (tableEmployee == null)
            {
                return NotFound();
            }

            if (tableEmployee.StoreId.ToString() != User.FindFirst(ClaimTypes.NameIdentifier).Value)
            {
                return Forbid();
            }

            if (!String.IsNullOrEmpty(tableEmployee.TelephoneNumber))
            {
                string outPhoneNumber;
                var a = ValidationHelper.ProoveValidPhoneNumber(tableEmployee.TelephoneNumber, out outPhoneNumber);
                //TODO: Meldung wenn nicht passt
                tableEmployee.TelephoneNumber = outPhoneNumber;
            }

            return View(tableEmployee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,TelephoneNumber,DefaultAnnotation,Active,Image,StoreId")]
            TableEmployee tableEmployee, IFormFile picture)
        {
            if (id != tableEmployee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (tableEmployee.StoreId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                    {
                        return Forbid();
                    }

                    var tempImage = _context.TblEmployees.AsNoTracking().FirstOrDefault(a => a.Id == tableEmployee.Id).Image;

                    tableEmployee.Image = CreateImage(picture);

                    if (tableEmployee.Image == null)
                        tableEmployee.Image = tempImage;

                    if (!String.IsNullOrEmpty(tableEmployee.TelephoneNumber))
                    {
                        string outPhoneNumber;
                        var a = ValidationHelper.ProoveValidPhoneNumber(tableEmployee.TelephoneNumber, out outPhoneNumber);
                        //TODO: Meldung wenn nicht passt
                        tableEmployee.TelephoneNumber = outPhoneNumber;
                    }

                    _context.Update(tableEmployee);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "Daten erfolgreich gespeichert";
                    return RedirectToAction("Employees", "Stores");
                }
                catch (DbUpdateConcurrencyException)
                {
                    ViewData["message"] = "Daten erfolgreich gespeichert";
                    //if (!TableEmployeeExists(tableEmployee.Id))
                    //{
                    //    return NotFound();
                    //}
                    //else
                    //{
                    //    throw;
                    //}
                }
            }

            return View(tableEmployee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tableEmployee = await _context.TblEmployees.FirstOrDefaultAsync(m => m.Id == id);

            if (tableEmployee.StoreId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Forbid();
            }

            if (tableEmployee == null)
            {
                return NotFound();
            }

            return View(tableEmployee);
        }

        // POST: Employees/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            return RedirectToAction("Employees", "Stores");

            var tableEmployee = await _context.TblEmployees.FindAsync(id);

            if (tableEmployee.StoreId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Forbid();
            }

            var appointments = await _context.TblAppointments.Where(a => a.EmployeeId == id).ToListAsync();
            if (appointments != null && appointments.Any())
            {
                //TODO: Informieren, dass den Mitarbeiter nicht mehr gibt

                _context.TblAppointments.RemoveRange(appointments);
                await _context.SaveChangesAsync();
            }

            _context.TblEmployees.Remove(tableEmployee);
            await _context.SaveChangesAsync();
            return RedirectToAction("Employees", "Stores");
        }

        private byte[] CreateImage(IFormFile picture)
        {
            if (picture != null)
            {
                byte[] pictureData;

                using (var stream = picture.OpenReadStream())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        pictureData = ms.ToArray();
                    }
                }

                return ImageHelper.ReduceImage(pictureData);
            }

            return null;
        }

        private bool TableEmployeeExists(int id)
        {
            return _context.TblEmployees.Any(e => e.Id == id);
        }
    }
}