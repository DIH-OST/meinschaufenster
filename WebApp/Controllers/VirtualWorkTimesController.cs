// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;
using Database.Tables;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Shop")]
    public class VirtualWorkTimesController : Controller
    {
        private readonly Db _context;

        public VirtualWorkTimesController(Db context)
        {
            _context = context;
        }

        // GET: VirtualWorkTimes
        // A simple View is returned
        // The Grid will asyc call the VirtualWorkTimes_Read action on page load
        public IActionResult Index() => View();

        public async Task<IActionResult> TblVirtualWorkTimes_Read([DataSourceRequest] DataSourceRequest request)
        {
            var storeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var employeeId = await _context.TblEmployees.Where(x => x.StoreId == storeId).Select(e => e.Id).FirstOrDefaultAsync();

            return Json(await _context.TblVirtualWorkTimes.Where(x => x.EmployeeId == employeeId).ToDataSourceResultAsync(request));
        }

        // GET: VirtualWorkTimes/Create
        public async Task<IActionResult> Create()
        {
            var storeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var employeeId = await _context.TblEmployees.Where(x => x.StoreId == storeId).Select(e => e.Id).FirstOrDefaultAsync();

            var vwt = new TableVirtualWorkTime {EmployeeId = employeeId, TimeSlot = 15};
            return View(vwt);
        }

        // POST: VirtualWorkTimes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Weekday,TimeFrom,TimeTo,TimeSlot,EmployeeId")]
            TableVirtualWorkTime tableVirtualWorkTime)
        {
            if (ModelState.IsValid)
            {
                if (tableVirtualWorkTime.TimeSlot < 5)
                {
                    tableVirtualWorkTime.TimeSlot = 5;
                }

                _context.Add(tableVirtualWorkTime);
                await _context.SaveChangesAsync();
                return RedirectToAction("Employees", "Stores");
            }

            return View(tableVirtualWorkTime);
        }

        // GET: VirtualWorkTimes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tableVirtualWorkTime = await _context.TblVirtualWorkTimes.FindAsync(id);
            if (tableVirtualWorkTime == null)
            {
                return NotFound();
            }

            var storeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var employeeId = await _context.TblEmployees.Where(x => x.StoreId == storeId).Select(e => e.Id).FirstOrDefaultAsync();
            if (tableVirtualWorkTime.EmployeeId != employeeId)
            {
                return Forbid();
            }

            return View(tableVirtualWorkTime);
        }

        // POST: VirtualWorkTimes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Weekday,TimeFrom,TimeTo,TimeSlot,EmployeeId")]
            TableVirtualWorkTime tableVirtualWorkTime)
        {
            if (id != tableVirtualWorkTime.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var storeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    var employeeId = await _context.TblEmployees.Where(x => x.StoreId == storeId).Select(e => e.Id).FirstOrDefaultAsync();
                    if (tableVirtualWorkTime.EmployeeId != employeeId)
                    {
                        return Forbid();
                    }

                    if (tableVirtualWorkTime.TimeSlot < 5)
                    {
                        tableVirtualWorkTime.TimeSlot = 5;
                    }

                    _context.Update(tableVirtualWorkTime);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "Daten erfolgreich gespeichert";
                    return RedirectToAction("Employees", "Stores");
                }
                catch (DbUpdateConcurrencyException)
                {
                    ViewData["message"] = "Daten konnten nicht erfolgreich gespeichert werden";
                }
            }

            return View(tableVirtualWorkTime);
        }

        // GET: VirtualWorkTimes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tableVirtualWorkTime = await _context.TblVirtualWorkTimes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tableVirtualWorkTime == null)
            {
                return NotFound();
            }

            var storeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var employeeId = await _context.TblEmployees.Where(x => x.StoreId == storeId).Select(e => e.Id).FirstOrDefaultAsync();
            if (tableVirtualWorkTime.EmployeeId != employeeId)
            {
                return Forbid();
            }

            return View(tableVirtualWorkTime);
        }

        // POST: VirtualWorkTimes/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tableVirtualWorkTime = await _context.TblVirtualWorkTimes.FindAsync(id);

            var storeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var employeeId = await _context.TblEmployees.Where(x => x.StoreId == storeId).Select(e => e.Id).FirstOrDefaultAsync();
            if (tableVirtualWorkTime.EmployeeId != employeeId)
            {
                return Forbid();
            }

            _context.TblVirtualWorkTimes.Remove(tableVirtualWorkTime);
            await _context.SaveChangesAsync();
            TempData["message"] = "Datensatz erfolgreich gelöscht";
            return RedirectToAction("Employees", "Stores");
        }

        private bool TableVirtualWorkTimeExists(int id)
        {
            return _context.TblVirtualWorkTimes.Any(e => e.Id == id);
        }
    }
}