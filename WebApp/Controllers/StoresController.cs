// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;
using Database.Tables;
using Exchange;
using Exchange.Enum;
using Exchange.Helper;
using Exchange.Model;
using Exchange.PostRequests;
using Exchange.ServiceAccess;
using WebApp.Models;
using WebApp.Services;
using WebRestApi.Helper;

namespace WebApp.Controllers
{
    public class StoresController : Controller
    {
        private readonly Db _context;

        readonly UserManager _userManager;

        public StoresController(UserManager userManager, Db context)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Stores
        // A simple View is returned
        // The Grid will asyc call the Stores_Read action on page load
        public IActionResult Index() => View();

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TblStores_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(await _context.TblStores.ToDataSourceResultAsync(request));
        }

        [Authorize(Roles = "Admin")]
        // GET: Stores/Create
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SignInAsShop(int shopId)
        {
            await _userManager.SignInShopForAdmin(HttpContext, shopId);
            return RedirectToAction("Index", "Appointments");
        }

        [Authorize(Roles = "Admin")]
        // POST: Stores/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CompanyName,Description,EMail,Website,ActivationCode,Activated,CreatedAt,PostCode,City,Address,Country,FederalState,Latitude,Longitude,Telephonenumber,Password,RestPassword")]
            TableStore tableStore, IFormFile logoFile)
        {
            if (ModelState.IsValid)
            {
                var logo = CreateImage(logoFile, EnumImageType.Logo);

                if (!(logo is null))
                {
                    logo.Store = tableStore;
                    _context.TblImages.Add(logo);
                }

                _context.Add(tableStore);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(tableStore);
        }

        [Authorize(Roles = "Admin")]
        // GET: Stores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tableStore = await _context.TblStores.FindAsync(id);

            if (tableStore == null)
            {
                return NotFound();
            }

            return View(tableStore);
        }

        [Authorize(Roles = "Admin")]
        // POST: Stores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CompanyName,Description,EMail,Website,ActivationCode,Activated,CreatedAt,PostCode,City,Address,Country,FederalState,Latitude,Longitude,Telephonenumber,Password,RestPassword")]
            TableStore tableStore, IFormFile logoFile)
        {
            if (id != tableStore.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var logo = CreateImage(logoFile, EnumImageType.Logo);

                if (!(logo is null))
                {
                    var storeImages = _context.TblStores.Where(s => s.Id == id).Select(s => s.Images).ToArray();
                    logo.Store = tableStore;
                    if (storeImages[0].Any())
                    {
                        for (int i = 0; i < storeImages[0].Count(); i++)
                        {
                            if (storeImages[0][i].ImageType == EnumImageType.Logo)
                            {
                                _context.TblImages.Remove(storeImages[0][i]);
                                break;
                            }
                        }
                    }

                    _context.TblImages.Add(logo);
                }

                try
                {
                    _context.Update(tableStore);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TableStoreExists(tableStore.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(tableStore);
        }

        [Authorize(Roles = "Admin")]
        // GET: Stores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tableStore = await _context.TblStores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tableStore == null)
            {
                return NotFound();
            }

            return View(tableStore);
        }

        [HttpPost]
        [Authorize(Roles = "Shop")]
        public async Task<IActionResult> DeleteStore()
        {
            //Geschäft löschen
            var storeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            RestAccess ra = new RestAccess(Constants.ServiceClientEndPointWithApiPrefix);
            var res = await ra.DeleteShop(new ExDeleteRequest
                                          {
                                              Id = storeId,
                                              CheckPassword = WebAppSettings.CheckPassword,
                                          });

            if (res.Ok)
            {
                if (!res.Result)
                {
                    TempData["message"] = "Geschäft konnte nicht gelöscht werden";
                    return RedirectToAction(nameof(StoreData));
                }

                //user ausloggen
                return RedirectToAction("Logout", "Account");
            }

            TempData["message"] = "Geschäft konnte nicht gelöscht werden";
            return RedirectToAction(nameof(StoreData));
        }

        [Authorize(Roles = "Shop")]
        // POST: Stores/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var res = new RestAccess("", "", Constants.ServiceClientEndPointWithApiPrefix);
            await res.DeleteShop(new ExDeleteRequest
                                 {
                                     Id = id,
                                     CheckPassword = WebAppSettings.CheckPassword,
                                 });

            return RedirectToAction("Logout", "Account");
        }

        private TableImage CreateImage(IFormFile logoFile, EnumImageType imageType)
        {
            if (logoFile != null && (logoFile.ContentType == "image/png" || logoFile.ContentType == "image/jpeg"))
            {
                var image = new TableImage
                            {
                                ImageType = imageType,
                                FileName = logoFile.FileName,
                                SavedAt = DateTime.Now,
                            };

                byte[] imageData;

                using (var stream = logoFile.OpenReadStream())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        imageData = ms.ToArray();
                    }
                }

                if (imageData.Length == 0)
                {
                    return null;
                }

                image.File = ImageHelper.ReduceImage(imageData);

                return image;
            }

            return null;
        }

        private bool TableStoreExists(int id)
        {
            return _context.TblStores.Any(e => e.Id == id);
        }

        #region Stammdaten

        // GET: Stores/StoreData
        [Authorize(Roles = "Shop")]
        public async Task<IActionResult> StoreData()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewData["message"] = TempData["message"];
            }

            var storeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var tableStore = await _context.TblStores.FindAsync(storeId);
            if (tableStore == null)
            {
                return NotFound();
            }

            tableStore.Password = "";
            tableStore.RestPassword = "";
            return View(tableStore);
        }

        // POST: Stores/StoreData/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Shop")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StoreData([Bind("Id,CompanyName,Description,EMail,Website,Activated,CreatedAt,PostCode,City,Address,Country,FederalState,Telephonenumber,Password")]
            TableStore tableStore, IFormFile logoFile)
        {
            var storeId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (ModelState.IsValid)
            {
                //update
                try
                {
                    var tbStore = await _context.TblStores.FirstOrDefaultAsync(x => x.Id == tableStore.Id);


                    var logo = CreateImage(logoFile, EnumImageType.Logo);
                    if (!(logo is null))
                    {
                        var storeImages = _context.TblStores.Where(s => s.Id.ToString() == storeId).Select(s => s.Images).ToArray();
                        logo.Store = tbStore;
                        if (storeImages[0].Any())
                        {
                            for (int i = 0; i < storeImages[0].Count(); i++)
                            {
                                if (storeImages[0][i].ImageType == EnumImageType.Logo)
                                {
                                    _context.TblImages.Remove(storeImages[0][i]);
                                    break;
                                }
                            }
                        }

                        _context.TblImages.Add(logo);
                    }

                    if (tbStore == null)
                    {
                        return NotFound();
                    }

                    if (!String.IsNullOrEmpty(tableStore.Telephonenumber))
                    {
                        string outPhoneNumber;
                        var a = ValidationHelper.ProoveValidPhoneNumber(tableStore.Telephonenumber, out outPhoneNumber);
                        //TODO: Meldung wenn nicht passt
                        tableStore.Telephonenumber = outPhoneNumber;
                    }

                    if (!String.IsNullOrEmpty(tableStore.Website))
                    {
                        if (!tableStore.Website.StartsWith("http"))
                        {
                            tableStore.Website = $"http://{tableStore.Website}";
                        }
                    }

                    tbStore.CompanyName = tableStore.CompanyName;
                    tbStore.Description = tableStore.Description;
                    tbStore.EMail = tableStore.EMail;
                    tbStore.Country = tableStore.Country;
                    tbStore.FederalState = tableStore.FederalState;
                    tbStore.PostCode = tableStore.PostCode;
                    tbStore.City = tableStore.City;
                    tbStore.Address = tableStore.Address;
                    tbStore.Website = tableStore.Website;
                    tbStore.Telephonenumber = tableStore.Telephonenumber;

                    var sec = await _context.TblLocations.FirstOrDefaultAsync(a => a.StoreId == tbStore.Id);
                    if (sec == null)
                    {
                        sec = new TableLocation();
                        sec.StoreId = tbStore.Id;
                        _context.TblLocations.Add(sec);
                    }

                    sec.EMail = tbStore.EMail;
                    sec.Address = tbStore.Address;
                    sec.City = tbStore.City;
                    sec.Country = tbStore.Country;
                    sec.PostCode = tbStore.PostCode;
                    sec.Name = tbStore.CompanyName;
                    sec.Telephonenumber = tbStore.Telephonenumber;

                    try
                    {
                        var res = GeocodeService.ConvertToGPSCoordinates(tableStore.PostCode, tableStore.City, tableStore.Address);
                        tbStore.Longitude = res.Geometry.Location.Longitude;
                        tbStore.Latitude = res.Geometry.Location.Latitude;
                    }
                    catch (Exception e)
                    {
                    }

                    sec.Latitude = tbStore.Latitude;
                    sec.Longitude = tbStore.Longitude;

                    if (!string.IsNullOrWhiteSpace(tableStore.Password))
                    {
                        tbStore.Password = PasswordHelper.CumputeHash(tableStore.Password);
                    }

                    await _context.SaveChangesAsync();

                    TempData.Add("message", "Daten erfolgreich gespeichert");
                    return RedirectToAction(nameof(StoreData));
                }
                catch (DbUpdateConcurrencyException)
                {
                    ViewData.Add("message", "Daten konnten nicht gespeichert werden");
                    return View(tableStore);
                    //if (!TableStoreExists(store.Id))
                    //{
                    //    return NotFound();
                    //}
                    //else
                    //{
                    //    throw;
                    //}
                }
            }

            return View(tableStore);
        }


        [Authorize(Roles = "Shop")]
        public async Task<IActionResult> OpeningHours()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewData["message"] = TempData["message"];
            }

            return View();
        }


        [Authorize(Roles = "Shop")]
        public async Task<IActionResult> ProductCategories()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewData["message"] = TempData["message"];
            }

            var storeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var store = await _context.TblStores
                .Include(x => x.TblStoreCategories)
                .Include(x => x.TblStoreCategories).ThenInclude(x => x.TblProductCategory)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == storeId);

            var categories = await _context.TblProductCategories
                .Include(x => x.TblStoreCategory)
                .Include(x => x.TblStoreCategory).ThenInclude(x => x.TblStore)
                .AsNoTracking()
                .Select(x => new ExCategoryItem
                             {
                                 Id = x.Id,
                                 Name = x.Description,
                                 Glyph = x.Icon,
                                 checkboxAnswer = store.TblStoreCategories != null && store.TblStoreCategories.FirstOrDefault(y => y.TblProductCategoryId == x.Id) != null
                             }).ToListAsync();

            var mainCategory = store.TblStoreCategories.FirstOrDefault(x => x.IsMainStoreCategory);

            var model = new CategoriesModel
                        {
                            SelectedCategories = categories,
                            MainCategoryId = categories.FirstOrDefault(x => mainCategory == null || x.Name == mainCategory.TblProductCategory.Description)?.Id ?? -1,
                        };

            return View(model);
        }

        [Authorize(Roles = "Shop")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductCategories(CategoriesModel model)
        {
            var storeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var store = await _context.TblStores
                .Include(x => x.TblStoreCategories)
                .Include(x => x.TblStoreCategories).ThenInclude(x => x.TblProductCategory)
                .FirstAsync(x => x.Id == storeId);

            try
            {
                // delete old entries
                if (store.TblStoreCategories == null) store.TblStoreCategories = new List<TableStoreCategory>();

                if (store.TblStoreCategories.Any())
                {
                    _context.TblStoreCategory.RemoveRange(store.TblStoreCategories);
                }

                store.TblStoreCategories.Clear();
                var changes = await _context.SaveChangesAsync();

                // add all selected ones again
                var ids = model.SelectedCategories.Where(x => x.checkboxAnswer).Select(x => x.Id).ToList();

                var mainCategory = _context.TblProductCategories.FirstOrDefault(x => x.Id == model.MainCategoryId);
                if (mainCategory != null)
                {
                    store.TblStoreCategories.Add(new TableStoreCategory
                                                 {
                                                     IsMainStoreCategory = true,
                                                     TblProductCategory = mainCategory,
                                                 });
                }

                var categories = _context.TblProductCategories.Where(x => ids.Contains(x.Id) && x.Id != model.MainCategoryId).ToList();

                store.TblStoreCategories.AddRange(categories.Select(x => new TableStoreCategory
                                                                         {
                                                                             IsMainStoreCategory = false,
                                                                             TblProductCategory = x,
                                                                         }));
                changes = await _context.SaveChangesAsync();
                TempData.Add("message", "Daten erfolgreich gespeichert");
                return RedirectToAction(nameof(ProductCategories));
            }
            catch (DbUpdateConcurrencyException)
            {
                ViewData.Add("message", "Daten konnten nicht gespeichert werden");
                return View(model);
                //if (!TableStoreExists(store.Id))
                //{
                //    return NotFound();
                //}
                //else
                //{
                //    throw;
                //}
            }
        }


        [Authorize(Roles = "Shop")]
        public async Task<IActionResult> DeliveryOptions()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewData["message"] = TempData["message"];
            }

            var storeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var store = await _context.TblStores.Include(a => a.TblStoreDelivery).FirstOrDefaultAsync(x => x.Id == storeId);

            var categories = await _context.TblDeliveryOptions.Include("TblStores").Select(x => new ExCategoryItem
                                                                                                {
                                                                                                    Id = x.Id,
                                                                                                    Name = x.Description,
                                                                                                    Glyph = x.Icon,
                                                                                                    checkboxAnswer = store.TblStoreDelivery != null && store.TblStoreDelivery.FirstOrDefault(y => y.TblDeliveryOptionId == x.Id) != null
                                                                                                }).ToListAsync();

            return View(categories);
        }

        [Authorize(Roles = "Shop")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeliveryOptions(List<ExCategoryItem> items)
        {
            var storeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (!ModelState.IsValid)
            {
                return View(items);
            }

            TableStore store = await _context.TblStores.Include(a => a.TblStoreDelivery).FirstAsync(x => x.Id == storeId);
            try
            {
                // delete old entries
                if (store.TblStoreDelivery == null) store.TblStoreDelivery = new List<TableStoreDelivery>();

                if (store.TblStoreDelivery.Any())
                {
                    _context.TblStoreDelivery.RemoveRange(store.TblStoreDelivery);
                }

                store.TblStoreDelivery.Clear();
                var changes = await _context.SaveChangesAsync();

                // add all selected ones again
                var ids = items.Where(x => x.checkboxAnswer).Select(x => x.Id).ToList();
                var deliveryOptions = _context.TblDeliveryOptions.Where(x => ids.Contains(x.Id)).ToList();

                store.TblStoreDelivery.AddRange(deliveryOptions.Select(x => new TableStoreDelivery
                                                                            {
                                                                                TblDeliveryOption = x
                                                                            }));
                changes = await _context.SaveChangesAsync();
                TempData.Add("message", "Daten erfolgreich gespeichert");
                return RedirectToAction(nameof(DeliveryOptions));
            }
            catch (DbUpdateConcurrencyException)
            {
                ViewData.Add("message", "Daten konnten nicht gespeichert werden");
                return View(items);
                //if (!TableStoreExists(store.Id))
                //{
                //    return NotFound();
                //}
                //else
                //{
                //    throw;
                //}
            }
        }


        [Authorize(Roles = "Shop")]
        public async Task<IActionResult> PaymentOptions()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewData["message"] = TempData["message"];
            }

            var storeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var store = await _context.TblStores.Include(a => a.TblStorePayments).FirstOrDefaultAsync(x => x.Id == storeId);

            var paymentOptions = await _context.TblPaymentOptions.Include("TblStores").Select(x => new ExCategoryItem
                                                                                                   {
                                                                                                       Id = x.Id,
                                                                                                       Name = x.Description,
                                                                                                       Glyph = x.Icon,
                                                                                                       checkboxAnswer = store.TblStorePayments != null && store.TblStorePayments.FirstOrDefault(y => y.TblPaymentOptionId == x.Id) != null
                                                                                                   }).ToListAsync();

            return View(paymentOptions);
        }

        [Authorize(Roles = "Shop")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaymentOptions(List<ExCategoryItem> items)
        {
            var storeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (!ModelState.IsValid)
            {
                return View(items);
            }

            TableStore store = await _context.TblStores.Include(a => a.TblStorePayments).FirstAsync(x => x.Id == storeId);
            try
            {
                // delete old entries
                if (store.TblStorePayments == null) store.TblStorePayments = new List<TableStorePayment>();

                if (store.TblStorePayments.Any())
                {
                    _context.TblStorePayment.RemoveRange(store.TblStorePayments);
                }

                store.TblStorePayments.Clear();

                var changes = await _context.SaveChangesAsync();

                // add all selected ones again
                var ids = items.Where(x => x.checkboxAnswer).Select(x => x.Id).ToList();
                var paymentOptions = _context.TblPaymentOptions.Where(x => ids.Contains(x.Id)).ToList();

                store.TblStorePayments.AddRange(paymentOptions.Select(x => new TableStorePayment
                                                                           {
                                                                               TblPaymentOption = x
                                                                           }));
                changes = await _context.SaveChangesAsync();
                TempData.Add("message", "Daten erfolgreich gespeichert");
                return RedirectToAction(nameof(PaymentOptions));
            }
            catch (DbUpdateConcurrencyException)
            {
                ViewData.Add("message", "Daten konnten nicht gespeichert werden");
                return View(items);
                //if (!TableStoreExists(store.Id))
                //{
                //    return NotFound();
                //}
                //else
                //{
                //    throw;
                //}
            }
        }

        [Authorize(Roles = "Shop")]
        public async Task<IActionResult> Employees()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewData["message"] = TempData["message"];
            }

            return View();
        }

        #endregion
    }
}