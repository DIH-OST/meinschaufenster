// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;
using Database.Tables;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Shop")]
    public class OpeningHoursController : Controller
    {
        private readonly Db _context;

        public OpeningHoursController(Db context)
        {
            _context = context;
        }

        // The Grid will asyc call the OpeningHours_Read action on page load
        public async Task<IActionResult> TblOpeningHours_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(await _context.TblOpeningHours.Where(x => x.StoreId.ToString() == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToDataSourceResultAsync(request));
        }

        // GET: OpeningHours/Create
        public IActionResult Create()
        {
            var hours = new TableOpeningHours();
            hours.StoreId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return View(hours);
        }

        // POST: OpeningHours/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Weekday,TimeFrom,TimeTo,TimeFromLocal,TimeToLocal,StoreId")]
            TableOpeningHours tableOpeningHours)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(tableOpeningHours);
                    await _context.SaveChangesAsync();
                    TempData.Add("message", "Daten erfolgreich gespeichert");
                    return RedirectToAction("OpeningHours", "Stores");
                }
                catch (Exception)
                {
                    ViewData.Add("message", "Daten konnten nicht gespeichert werden");
                }
            }

            return View(tableOpeningHours);
        }

        // GET: OpeningHours/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tableOpeningHours = await _context.TblOpeningHours.FindAsync(id);
            if (tableOpeningHours == null)
            {
                return NotFound();
            }

            if (tableOpeningHours.StoreId.ToString() != User.FindFirst(ClaimTypes.NameIdentifier).Value)
            {
                return Forbid();
            }

            return View(tableOpeningHours);
        }

        // POST: OpeningHours/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Weekday,TimeFrom,TimeTo,TimeFromLocal,TimeToLocal,StoreId")]
            TableOpeningHours tableOpeningHours)
        {
            if (id != tableOpeningHours.Id)
            {
                return NotFound();
            }

            if (tableOpeningHours.StoreId.ToString() != User.FindFirst(ClaimTypes.NameIdentifier).Value)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tableOpeningHours);
                    await _context.SaveChangesAsync();
                    TempData.Add("message", "Daten erfolgreich gespeichert");
                    return RedirectToAction("OpeningHours", "Stores");
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData.Add("message", "Daten konnten nicht gespeichert werden");
                    //if (!TableOpeningHoursExists(tableOpeningHours.Id))
                    //{
                    //    return NotFound();
                    //}
                    //else
                    //{
                    //    throw;
                    //}
                }
            }

            return View(tableOpeningHours);
        }

        // GET: OpeningHours/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tableOpeningHours = await _context.TblOpeningHours
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tableOpeningHours == null)
            {
                return NotFound();
            }

            return View(tableOpeningHours);
        }

        // POST: OpeningHours/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tableOpeningHours = await _context.TblOpeningHours.FindAsync(id);
            _context.TblOpeningHours.Remove(tableOpeningHours);
            await _context.SaveChangesAsync();
            return RedirectToAction("OpeningHours", "Stores");
        }

        private bool TableOpeningHoursExists(int id)
        {
            return _context.TblOpeningHours.Any(e => e.Id == id);
        }
    }
}