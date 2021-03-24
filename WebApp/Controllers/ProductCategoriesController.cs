// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;
using Database.Tables;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductCategoriesController : Controller
    {
        private readonly Db _context;

        public ProductCategoriesController(Db context)
        {
            _context = context;
        }

        // GET: ProductCategories
        // A simple View is returned
        // The Grid will asyc call the ProductCategories_Read action on page load
        public IActionResult Index() => View();

        public async Task<IActionResult> TblProductCategories_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(await _context.TblProductCategories.ToDataSourceResultAsync(request));
        }

        // GET: ProductCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProductCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Icon")] TableProductCategory tableProductCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tableProductCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(tableProductCategory);
        }

        // GET: ProductCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tableProductCategory = await _context.TblProductCategories.FindAsync(id);
            if (tableProductCategory == null)
            {
                return NotFound();
            }

            return View(tableProductCategory);
        }

        // POST: ProductCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Icon")] TableProductCategory tableProductCategory)
        {
            if (id != tableProductCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tableProductCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TableProductCategoryExists(tableProductCategory.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(tableProductCategory);
        }

        // GET: ProductCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tableProductCategory = await _context.TblProductCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tableProductCategory == null)
            {
                return NotFound();
            }

            return View(tableProductCategory);
        }

        // POST: ProductCategories/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tableProductCategory = await _context.TblProductCategories.FindAsync(id);

            if (_context.TblStores.Any(s => s.TblStoreCategories.Any(c => c.TblProductCategoryId == id)))
            {
                ViewData["message"] = "Sie haben keine Berechtigung für diese Aktion";
                return View(tableProductCategory);
            }

            _context.TblProductCategories.Remove(tableProductCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TableProductCategoryExists(int id)
        {
            return _context.TblProductCategories.Any(e => e.Id == id);
        }
    }
}