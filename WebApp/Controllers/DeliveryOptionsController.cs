// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;
using Database.Tables;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DeliveryOptionsController : Controller
    {
        private readonly Db _context;

        public DeliveryOptionsController(Db context)
        {
            _context = context;
        }

        // GET: DeliveryOptions
        // A simple View is returned
        // The Grid will asyc call the DeliveryOptions_Read action on page load
        public IActionResult Index() => View();

        public async Task<IActionResult> TblDeliveryOptions_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(await _context.TblDeliveryOptions.ToDataSourceResultAsync(request));
        }

        // GET: DeliveryOptions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DeliveryOptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Icon")] TableDeliveryOption tableDeliveryOption)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tableDeliveryOption);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(tableDeliveryOption);
        }

        // GET: DeliveryOptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tableDeliveryOption = await _context.TblDeliveryOptions.FindAsync(id);
            if (tableDeliveryOption == null)
            {
                return NotFound();
            }

            return View(tableDeliveryOption);
        }

        // POST: DeliveryOptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Icon")] TableDeliveryOption tableDeliveryOption)
        {
            if (id != tableDeliveryOption.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tableDeliveryOption);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TableDeliveryOptionExists(tableDeliveryOption.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(tableDeliveryOption);
        }

        // GET: DeliveryOptions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tableDeliveryOption = await _context.TblDeliveryOptions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tableDeliveryOption == null)
            {
                return NotFound();
            }

            return View(tableDeliveryOption);
        }

        // POST: DeliveryOptions/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tableDeliveryOption = await _context.TblDeliveryOptions.FindAsync(id);

            if (_context.TblStores.Any(s => s.TblStoreDelivery.Any(c => c.TblDeliveryOptionId == id)))
            {
                ViewData["message"] = "Sie haben keine Berechtigung für diese Aktion";
                return View(tableDeliveryOption);
            }

            _context.TblDeliveryOptions.Remove(tableDeliveryOption);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TableDeliveryOptionExists(int id)
        {
            return _context.TblDeliveryOptions.Any(e => e.Id == id);
        }
    }
}