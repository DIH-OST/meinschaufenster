// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;
using Database.Tables;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PaymentOptionsController : Controller
    {
        private readonly Db _context;

        public PaymentOptionsController(Db context)
        {
            _context = context;
        }

        // GET: PaymentOptions
        // A simple View is returned
        // The Grid will asyc call the PaymentOptions_Read action on page load
        public IActionResult Index() => View();

        public async Task<IActionResult> TblPaymentOptions_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(await _context.TblPaymentOptions.ToDataSourceResultAsync(request));
        }

        // GET: PaymentOptions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PaymentOptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Icon")] TablePaymentOption tablePaymentOption)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tablePaymentOption);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(tablePaymentOption);
        }

        // GET: PaymentOptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tablePaymentOption = await _context.TblPaymentOptions.FindAsync(id);
            if (tablePaymentOption == null)
            {
                return NotFound();
            }

            return View(tablePaymentOption);
        }

        // POST: PaymentOptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Icon")] TablePaymentOption tablePaymentOption)
        {
            if (id != tablePaymentOption.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tablePaymentOption);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TablePaymentOptionExists(tablePaymentOption.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(tablePaymentOption);
        }

        // GET: PaymentOptions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tablePaymentOption = await _context.TblPaymentOptions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tablePaymentOption == null)
            {
                return NotFound();
            }

            return View(tablePaymentOption);
        }

        // POST: PaymentOptions/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tablePaymentOption = await _context.TblPaymentOptions.FindAsync(id);

            if (_context.TblStores.Any(s => s.TblStorePayments.Any(c => c.TblPaymentOptionId == id)))
            {
                ViewData["message"] = "Sie haben keine Berechtigung für diese Aktion";
                return View(tablePaymentOption);
            }

            _context.TblPaymentOptions.Remove(tablePaymentOption);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TablePaymentOptionExists(int id)
        {
            return _context.TblPaymentOptions.Any(e => e.Id == id);
        }
    }
}