// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;
using Database.Tables;
using Exchange;
using Exchange.Enum;
using Exchange.Helper;
using Exchange.Model;
using Exchange.PostRequests;
using Exchange.Resources;
using WebRestApi.Helper;
using WebRestApi.Services;

namespace WebRestApi.Controllers
{
    /// <summary>
    ///     <para>Shopcontroller</para>
    ///     Klasse ShopController. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [ApiController]
    public class ShopController : Controller
    {
        private readonly HtmlGenerator _mailgenerator;
        private readonly ViewRender _view;

        public ShopController(ViewRender view)
        {
            _view = view;
            _mailgenerator = new HtmlGenerator(view);
        }

        /// <summary>
        ///     Einen Shop komplett löschen
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("api/DeleteShop/")]
        public async Task<bool> DeleteShop([FromBody] ExDeleteRequest request)
        {
            var shopId = request.Id;

            if (request.CheckPassword != WebAppSettings.CheckPassword)
            {
                HttpContext.Response.StatusCode = Unauthorized().StatusCode;
                return false;
            }

            using (var db = new Db())
            {
                //Kundentermine absagen
                var appointments = db.TblAppointments
                    .Include(x => x.Employee)
                    .Where(a => a.Employee.StoreId == shopId);
                var appointmentsToInformForDelete = appointments.Where(a => a.ValidFrom > DateTime.UtcNow && a.Canceled == false);
                var mc = new MeetingController(_view);
                foreach (var appointment in appointmentsToInformForDelete)
                {
                    await mc.DeleteMeetingWeb(new ExRemoveMeetingRequest
                                              {
                                                  UserType = EnumUserType.ShopEmployee,
                                                  MeetingId = appointment.Id,
                                                  CheckPassword = WebAppSettings.CheckPassword,
                                                  UserId = -1
                                              });
                }

                //Kundentermine löschen
                db.TblAppointments.RemoveRange(appointments);

                //Mitarbeiter löschen
                var emp = db.TblEmployees
                    .Include(x => x.TblLocationEmployee).Where(e => e.StoreId == shopId);
                foreach (var e in emp)
                {
                    db.TblLocationEmployee.RemoveRange(e.TblLocationEmployee);
                }

                db.TblEmployees.RemoveRange(emp);

                //Shop
                var shop = await db.TblStores.Include(i => i.TblLocations).Include(i => i.TblStoreDelivery).Include(i => i.TblStorePayments).Include(i => i.TblStoreCategories).FirstOrDefaultAsync(s => s.Id == shopId);

                if (shop == null)
                    return false;

                //Zwischentabellen
                db.TblLocations.RemoveRange(shop.TblLocations);
                db.TblStoreDelivery.RemoveRange(shop.TblStoreDelivery);
                db.TblStorePayment.RemoveRange(shop.TblStorePayments);
                db.TblStoreCategory.RemoveRange(shop.TblStoreCategories);


                //Admins informieren 
                List<string> eMails2Inform;
                if (Constants.CurrentAppSettings.AppConfigurationConstants == 0) //master
                {
                    eMails2Inform = new List<string>
                                    {
                                        "info@meinschaufenster.at"
                                    }; //ToDo: Wenn die Settings Tabelle existiert dann über die Settings Tabelle
                }
                else
                {
                    eMails2Inform = new List<string>
                                    {
                                        "biss@fotec.at"
                                    }; //ToDo: Wenn die Settings Tabelle existiert dann über die Settings Tabelle
                }

                string emailContent = $"Leider hat der Shop {shop.CompanyName} mit der Telefonnummer {shop.Telephonenumber} seinen Account gelöscht.";
                string email = _mailgenerator.GetMessageOnlyEmail(new ExEMailMessageOnly
                                                                  {
                                                                      Message = emailContent
                                                                  });
                BissEMail bm = new BissEMail(WebAppSettings.EmailCredentials);
                var res = await bm.SendHtmlEMail(Constants.SendEMailAs, eMails2Inform, "Kunde gelöscht.", email, Constants.SendEMailAsDisplayName);

                //Löschen
                db.TblStores.Remove(shop);

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Logging.Log.LogError($"Error deleting Shop: {shop.CompanyName} Id: {shopId}: {e}");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Listen für Filter laden
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("api/GetFilterInfos")]
        public async Task<ExFilterInfos> GetFilterInfos()
        {
            using (var db = new Db())
            {
                var res = new ExFilterInfos
                          {
                              Categories = db.TblProductCategories.AsNoTracking().Select(x => new ExCategory
                                                                                              {
                                                                                                  Id = x.Id,
                                                                                                  Name = x.Description,
                                                                                                  Glyph = x.Icon,
                                                                                              }).ToList(),
                              PaymentMethods = db.TblPaymentOptions.AsNoTracking().Select(x => new ExPaymentMethod
                                                                                               {
                                                                                                   Id = x.Id,
                                                                                                   Name = x.Description,
                                                                                                   Glyph = x.Icon,
                                                                                               }).ToList(),
                              DeliveryMethods = db.TblDeliveryOptions.AsNoTracking().Select(x => new ExDeliveryMethod
                                                                                                 {
                                                                                                     Id = x.Id,
                                                                                                     Name = x.Description,
                                                                                                     Glyph = x.Icon,
                                                                                                 }).ToList(),
                          };

                return res;
            }
        }

        /// <summary>
        ///     Alle Shops für einen Request holen
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("api/GetShops")]
        public async Task<List<ExShopShort>> GetShops([FromBody] ExGetShopsRequest request)
        {
            return ShopCache.GetShops(request, nameof(GlyphToIcon));


            var sw = new Stopwatch();
            sw.Start();

            using (var db = new Db())
            {
                var res = new List<ExShopShort>();

                var secondaryLocations = db.TblLocations
                    .Include(x => x.Store)
                    .Include(x => x.Store).ThenInclude(x => x.TblStoreCategories)
                    .Include(x => x.Store).ThenInclude(x => x.TblStoreCategories).ThenInclude(x => x.TblProductCategory)
                    .Include(x => x.Store).ThenInclude(x => x.TblStoreDelivery)
                    .Include(x => x.Store).ThenInclude(x => x.TblStoreDelivery).ThenInclude(x => x.TblDeliveryOption)
                    .Include(x => x.Store).ThenInclude(x => x.TblStorePayments)
                    .Include(x => x.Store).ThenInclude(x => x.TblStorePayments).ThenInclude(x => x.TblPaymentOption)
                    .Include(x => x.Store).ThenInclude(x => x.OpeningHours)
                    .Include(x => x.Store).ThenInclude(x => x.SpecialDays)
                    .Include(x => x.Store).ThenInclude(x => x.Absences)
                    .Include(x => x.TblLocationEmployee)
                    .Include(x => x.TblLocationEmployee).ThenInclude(x => x.TblEmployee)
                    .Include(x => x.TblLocationEmployee).ThenInclude(x => x.TblEmployee).ThenInclude(x => x.TblVirtualWorkTimes)
                    .AsNoTracking()
                    .Where(x => x.Store.Activated);

                if (request.MyPosition != null)
                {
                    // TODO Ranges richtig setzen

                    var minLat = request.MyPosition.Latitude - request.Range;
                    var maxLat = request.MyPosition.Latitude + request.Range;

                    var minLon = request.MyPosition.Longitude - request.Range;
                    var maxLon = request.MyPosition.Longitude + request.Range;

                    secondaryLocations = secondaryLocations.Where(x =>
                        x.Latitude < maxLat && x.Latitude > minLat &&
                        x.Longitude < maxLon && x.Longitude > minLon);
                }

                foreach (var location in secondaryLocations)
                {
                    var shop = new ExShopShort
                               {
                                   Id = location.Id,
                                   Name = location.Store.CompanyName,
                                   Position = new BissPosition(location.Latitude, location.Longitude),
                                   MainCategory = location.Store.TblStoreCategories.FirstOrDefault(x => x.IsMainStoreCategory) != null
                                       ? new ExCategory
                                         {
                                             Id = location.Store.TblStoreCategories.FirstOrDefault(x => x.IsMainStoreCategory).TblProductCategory.Id,
                                             Name = location.Store.TblStoreCategories.FirstOrDefault(x => x.IsMainStoreCategory).TblProductCategory.Description,
                                             Glyph = location.Store.TblStoreCategories.FirstOrDefault(x => x.IsMainStoreCategory).TblProductCategory.Icon,
                                         }
                                       : location.Store.TblStoreCategories?.FirstOrDefault() != null
                                           ? new ExCategory
                                             {
                                                 Id = location.Store.TblStoreCategories.FirstOrDefault().TblProductCategory.Id,
                                                 Name = location.Store.TblStoreCategories.FirstOrDefault().TblProductCategory.Description,
                                                 Glyph = location.Store.TblStoreCategories.FirstOrDefault().TblProductCategory.Icon,
                                             }
                                           : null,
                                   Categories = location.Store.TblStoreCategories != null
                                       ? location.Store.TblStoreCategories.Select(x => new ExCategory
                                                                                       {
                                                                                           Id = x.TblProductCategory.Id,
                                                                                           Name = x.TblProductCategory.Description,
                                                                                           Glyph = x.TblProductCategory.Icon,
                                                                                       }).ToList()
                                       : new List<ExCategory>(),
                                   DeliveryMethods = location.Store.TblStoreDelivery != null
                                       ? location.Store.TblStoreDelivery.Select(x => new ExDeliveryMethod
                                                                                     {
                                                                                         Id = x.TblDeliveryOption.Id,
                                                                                         Name = x.TblDeliveryOption.Description,
                                                                                         Glyph = x.TblDeliveryOption.Icon,
                                                                                     }).ToList()
                                       : new List<ExDeliveryMethod>(),
                                   PaymentMethods = location.Store.TblStorePayments != null
                                       ? location.Store.TblStorePayments.Select(x => new ExPaymentMethod
                                                                                     {
                                                                                         Id = x.TblPaymentOption.Id,
                                                                                         Name = x.TblPaymentOption.Description,
                                                                                         Glyph = x.TblPaymentOption.Icon,
                                                                                     }).ToList()
                                       : new List<ExPaymentMethod>(),
                               };

                    #region Ist geöffnet

                    var abscence = location.Store.Absences.FirstOrDefault(x => x.Date == DateTime.UtcNow.Date);

                    // Schauen ob genereller Urlaubstag etc.
                    if (abscence != null)
                    {
                        shop.IsOpen = false;
                    }
                    else
                    {
                        // normale Öffnungszeiten
                        var openeningHours = location.Store.OpeningHours.FirstOrDefault(x => x.Weekday == DateTime.UtcNow.Date.DayOfWeek);

                        // sondertag der offen ist
                        var specialDays = location.Store.SpecialDays.FirstOrDefault(x => x.Date == DateTime.UtcNow.Date);

                        var isOpenThisDay = specialDays != null || openeningHours?.TimeFrom != null;

                        if (isOpenThisDay)
                        {
                            var currentTime = new DateTime(1, 1, 2, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, DateTime.UtcNow.Second);

                            var timeFrom = new DateTime(1, 1, (openeningHours.TimeFrom?.Date < openeningHours.TimeTo?.Date ? 1 : 2), openeningHours?.TimeFrom?.Hour ?? 0, openeningHours?.TimeFrom?.Minute ?? 0, openeningHours?.TimeFrom?.Second ?? 0);
                            var timeTo = new DateTime(1, 1, 2, openeningHours?.TimeTo?.Hour ?? 23, openeningHours?.TimeTo?.Minute ?? 59, openeningHours?.TimeTo?.Second ?? 59);

                            var shopIsOpenNow = openeningHours == null || (currentTime >= timeFrom && currentTime <= timeTo);

                            shop.IsOpen = shopIsOpenNow;
                        }
                        else
                        {
                            shop.IsOpen = false;
                        }
                    }

                    #endregion

                    #region Icon

                    if (shop?.MainCategory == null)
                        shop.MainCategory = new ExCategory
                                            {
                                                Id = -1,
                                                Name = "-",
                                                Glyph = "0"
                                            };

                    if (string.IsNullOrWhiteSpace(shop?.MainCategory?.Glyph))
                        shop.MainCategory.Glyph = "0";

                    var iconDroid = Constants.ServiceClientEndPointWithApiPrefix + nameof(GlyphToIcon) + $"/{shop?.MainCategory?.Glyph}" +
                                    $"/{(shop.IsOpen ? Color.ForestGreen : Color.Crimson).ToArgb()}" +
                                    $"/{Color.White.ToArgb()}" +
                                    $"/{Color.Transparent.ToArgb()}" +
                                    "/128" +
                                    $"/{string.IsNullOrWhiteSpace(shop?.MainCategory?.Glyph)}";

                    var iconIos = Constants.ServiceClientEndPointWithApiPrefix + nameof(GlyphToIcon) + $"/{shop?.MainCategory?.Glyph}" +
                                  $"/{(shop.IsOpen ? Color.ForestGreen : Color.Crimson).ToArgb()}" +
                                  $"/{Color.White.ToArgb()}" +
                                  $"/{Color.Transparent.ToArgb()}" +
                                  "/50" +
                                  $"/{string.IsNullOrWhiteSpace(shop?.MainCategory?.Glyph)}";

                    shop.MainCategory.Pin = new BissPinInfo(shop.Position, shop.Name, iconDroid, iconIos);

                    #endregion

                    res.Add(shop);
                }

                Logging.Log.LogWarning("Finished " + sw.Elapsed);
                sw.Stop();

                return res;
            }
        }

        /// <summary>
        ///     Shopinfos holen
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("api/GetShopInfo/{locationId}")]
        public async Task<ExShop> GetShopInfo(int locationId)
        {
            var sw = new Stopwatch();
            sw.Start();

            using (var db = new Db())
            {
                Logging.Log.LogWarning("create db context " + sw.Elapsed);
                sw.Reset();
                sw.Start();

                var location = db.TblLocations
                    .Include(x => x.Store)
                    .Include(x => x.Store).ThenInclude(x => x.TblStoreCategories)
                    .Include(x => x.Store).ThenInclude(x => x.TblStoreCategories).ThenInclude(x => x.TblProductCategory)
                    .Include(x => x.Store).ThenInclude(x => x.TblStoreDelivery)
                    .Include(x => x.Store).ThenInclude(x => x.TblStoreDelivery).ThenInclude(x => x.TblDeliveryOption)
                    .Include(x => x.Store).ThenInclude(x => x.TblStorePayments)
                    .Include(x => x.Store).ThenInclude(x => x.TblStorePayments).ThenInclude(x => x.TblPaymentOption)
                    .Include(x => x.Store).ThenInclude(x => x.OpeningHours)
                    .Include(x => x.Store).ThenInclude(x => x.SpecialDays)
                    .Include(x => x.Store).ThenInclude(x => x.Absences)
                    .Include(x => x.TblLocationEmployee)
                    .Include(x => x.TblLocationEmployee).ThenInclude(x => x.TblEmployee)
                    .Include(x => x.TblLocationEmployee).ThenInclude(x => x.TblEmployee).ThenInclude(x => x.TblVirtualWorkTimes)
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == locationId);

                if (location == null)
                    return null;

                Logging.Log.LogWarning("linq " + sw.Elapsed);
                sw.Reset();
                sw.Start();

                var res = new ExShop
                          {
                              Id = location.Id,
                              Name = location.Store.CompanyName,
                              Position = new BissPosition(location.Latitude, location.Longitude),
                              MainCategory = location.Store.TblStoreCategories.FirstOrDefault(x => x.IsMainStoreCategory) != null
                                  ? new ExCategory
                                    {
                                        Id = location.Store.TblStoreCategories.FirstOrDefault(x => x.IsMainStoreCategory).TblProductCategory.Id,
                                        Name = location.Store.TblStoreCategories.FirstOrDefault(x => x.IsMainStoreCategory).TblProductCategory.Description,
                                        Glyph = location.Store.TblStoreCategories.FirstOrDefault(x => x.IsMainStoreCategory).TblProductCategory.Icon,
                                    }
                                  : location.Store.TblStoreCategories?.FirstOrDefault() != null
                                      ? new ExCategory
                                        {
                                            Id = location.Store.TblStoreCategories.FirstOrDefault().TblProductCategory.Id,
                                            Name = location.Store.TblStoreCategories.FirstOrDefault().TblProductCategory.Description,
                                            Glyph = location.Store.TblStoreCategories.FirstOrDefault().TblProductCategory.Icon,
                                        }
                                      : null,
                              Categories = location.Store.TblStoreCategories != null
                                  ? location.Store.TblStoreCategories.Select(x => new ExCategory
                                                                                  {
                                                                                      Id = x.TblProductCategory.Id,
                                                                                      Name = x.TblProductCategory.Description,
                                                                                      Glyph = x.TblProductCategory.Icon,
                                                                                  }).ToList()
                                  : new List<ExCategory>(),
                              DeliveryMethods = location.Store.TblStoreDelivery != null
                                  ? location.Store.TblStoreDelivery.Select(x => new ExDeliveryMethod
                                                                                {
                                                                                    Id = x.TblDeliveryOption.Id,
                                                                                    Name = x.TblDeliveryOption.Description,
                                                                                    Glyph = x.TblDeliveryOption.Icon,
                                                                                }).ToList()
                                  : new List<ExDeliveryMethod>(),
                              PaymentMethods = location.Store.TblStorePayments != null
                                  ? location.Store.TblStorePayments.Select(x => new ExPaymentMethod
                                                                                {
                                                                                    Id = x.TblPaymentOption.Id,
                                                                                    Name = x.TblPaymentOption.Description,
                                                                                    Glyph = x.TblPaymentOption.Icon,
                                                                                }).ToList()
                                  : new List<ExPaymentMethod>(),

                              PhoneNumber = location.Telephonenumber,
                              WebLink = location.Store.Website,
                              LocationName = location.Name,
                              Address = location.Address,
                              PostCode = location.PostCode,
                              City = location.City,
                              FederalState = location.FederalState,
                              Country = location.Country,
                              Employees = location.TblLocationEmployee != null
                                  ? location.TblLocationEmployee.Select(x => Staff.GetExStaff(x.TblEmployee)).ToList()
                                  : new List<ExStaff>(),
                              Description = location.Store.Description,
                              ImageUrl = Constants.ServiceClientEndPointWithApiPrefix + nameof(GetStoreImage) + "/" + location.StoreId,
                          };

                Logging.Log.LogWarning("stammdaten " + sw.Elapsed);

                #region Öffnungszeiten

                var openingHours = new List<ExOpeningHour>();

                for (var i = 0; i < 7; i++)
                {
                    var checkDate = DateTime.UtcNow.Date.AddDays(i);

                    var opening = location.Store.OpeningHours.FirstOrDefault(x => x.Weekday == checkDate.DayOfWeek);

                    var specialDay = location.Store.SpecialDays.FirstOrDefault(x => x.Date == checkDate);

                    var abscence = location.Store.Absences.FirstOrDefault(x => x.Date == checkDate);

                    openingHours.Add(new ExOpeningHour
                                     {
                                         Day = checkDate,
                                         TimeFrom = abscence != null || opening?.TimeFrom == null ? (DateTime?) null : DateTime.SpecifyKind(opening.TimeFrom.Value, DateTimeKind.Utc),
                                         TimeTo = abscence != null || opening?.TimeTo == null ? (DateTime?) null : DateTime.SpecifyKind(opening.TimeTo.Value, DateTimeKind.Utc),
                                         IsAbscenceDay = abscence != null,
                                         IsSpecialDay = specialDay != null,
                                     });
                }

                res.OpeningHours = openingHours;

                #endregion

                Logging.Log.LogWarning("+Öffnungszeiten " + sw.Elapsed);

                #region Mitarbeiterslots heute

                var slots = MeetingSlots.GetSlots(db, location.Id, DateTime.UtcNow);

                res.FreeSlots = slots.Where(x => x.Id < 0).ToList();

                #endregion

                Logging.Log.LogWarning("+Slots " + sw.Elapsed);

                #region Ist geöffnet

                if (openingHours.FirstOrDefault().IsAbscenceDay)
                {
                    res.IsOpen = false;
                }
                else if (openingHours.FirstOrDefault().IsSpecialDay)
                {
                    res.IsOpen = true;
                }
                else if (openingHours.FirstOrDefault().TimeFrom == null)
                {
                    res.IsOpen = false;
                }
                else
                {
                    var openeningHours = openingHours.FirstOrDefault();

                    var currentTime = new DateTime(1, 1, 2, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, DateTime.UtcNow.Second);

                    var timeFrom = new DateTime(1, 1, (openeningHours.TimeFrom?.Date < openeningHours.TimeTo?.Date ? 1 : 2), openeningHours?.TimeFrom?.Hour ?? 0, openeningHours?.TimeFrom?.Minute ?? 0, openeningHours?.TimeFrom?.Second ?? 0);
                    var timeTo = new DateTime(1, 1, 2, openeningHours?.TimeTo?.Hour ?? 23, openeningHours?.TimeTo?.Minute ?? 59, openeningHours?.TimeTo?.Second ?? 59);

                    var shopIsOpenNow = openeningHours == null || (currentTime >= timeFrom && currentTime <= timeTo);

                    res.IsOpen = shopIsOpenNow;

                    // Shop ist generell offen - verfügbarkeit checken
                    if (res.IsOpen)
                    {
                        foreach (var locationEmployee in location.TblLocationEmployee)
                        {
                            var workTimeToday = locationEmployee.TblEmployee.TblVirtualWorkTimes.FirstOrDefault(x => x.Weekday == DateTime.UtcNow.Date.DayOfWeek);

                            if (workTimeToday == null)
                                continue;

                            var meetings = db.TblAppointments.AsNoTracking()
                                .Where(x => x.EmployeeId == locationEmployee.Id && x.ValidTo >= currentTime && x.ValidFrom <= currentTime && !x.Canceled);

                            var meeting = meetings.FirstOrDefault();

                            if (meeting == null)
                            {
                                res.IsFree = true;
                                break;
                            }
                        }
                    }
                }

                #endregion

                Logging.Log.LogWarning("+isOpen " + sw.Elapsed);

                // TODO nexten Timeslot suchen und angeben ob frei oder besetzt
                res.NextSlot = DateTime.SpecifyKind(res.FreeSlots.FirstOrDefault()?.Start ?? DateTime.UtcNow.AddYears(1), DateTimeKind.Utc);
                res.WhatsappNumber = res.FreeSlots.FirstOrDefault()?.Staff?.WhatsappContact ??
                                     location.TblLocationEmployee?.FirstOrDefault()?.TblEmployee?.TelephoneNumber;

                Logging.Log.LogWarning("Finished " + sw.Elapsed);
                sw.Stop();

                return res;
            }
        }

        /// <summary>
        ///     Pin Image
        /// </summary>
        /// <param name="code"> zB "E994"</param>
        /// <param name="pinColor">Color.Crimson(rot): #FFDC143C  oder Color.ForestGreen: #FF228B22</param>
        /// <param name="glyphColor">Color.White: #FFFFFFFF</param>
        /// <param name="backgroundColor">Color.Transparent: #00FFFFFF</param>
        /// <param name="squaredSize">32 64 was auch immer</param>
        /// <param name="pinOnly">Soll nur ein Pin in der Farbe pinColor erzeugt werden?</param>
        /// <returns>PNG</returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("api/GlyphToIcon/{code}/{pinColor}/{glyphColor}/{backgroundColor}/{squaredSize}/{pinOnly}")]
        public ActionResult GlyphToIcon(string code, string pinColor, string glyphColor, string backgroundColor, int squaredSize, bool pinOnly)
        {
            var post = new ExGlyphDataPost
                       {
                           GlyphColor = ColorTranslator.FromHtml(glyphColor),
                           FontCode = char.ConvertFromUtf32(Convert.ToUInt16(code, 16)),
                           FontCodeNumber = code,
                           PinColor = ColorTranslator.FromHtml(pinColor),
                           PinOnly = pinOnly,
                           SquaredSize = squaredSize,
                           BackgroundColor = ColorTranslator.FromHtml(backgroundColor),
                       };

            var result = GlyphConverter.GetGlyph(post);
            return File(result.ImageData, "image/png");
        }

        /// <summary>
        ///     Bild für einen Shop laden
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("api/GetStoreImage/{storeId}")]
        public ActionResult GetStoreImage(int storeId)
        {
            using (var db = new Db())
            {
                var res = db.TblImages.FirstOrDefault(x => x.StoreId == storeId)?.File
                          ?? Images.ReadImage(EmbeddedImages.AppIconTransparent_png).Image;

                return File(res, "image/png");
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="exShopRegistration"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("api/RegisterShop")]
        public async Task<ExSaveDataResult> RegisterShop(ExShopRegistration exShopRegistration)
        {
            using (Db db = new Db())
            {
                var shop = await db.TblStores.FirstOrDefaultAsync(a => a.EMail.ToLower() == exShopRegistration.EMail.ToLower());

                if (shop == null)
                {
                    //NEU ANLEGEN

                    shop = new TableStore();
                    shop.EMail = exShopRegistration.EMail.ToLower();
                    shop.CreatedAt = DateTime.UtcNow;
                    shop.CompanyName = "";
                    db.TblStores.Add(shop);
                }
                else if (shop.Activated)
                {
                    return new ExSaveDataResult {Result = EnumSaveDataResult.Error};
                }


                string pwd = PasswordHelper.GeneratePassword(6);

                shop.Password = PasswordHelper.CumputeHash(pwd);
                shop.ActivationCode = PasswordHelper.GeneratePassword(10);
                await db.SaveChangesAsync();

                BissEMail bm = new BissEMail(WebAppSettings.EmailCredentials);

                ExEmailRegistration er = new ExEmailRegistration();

                er.Message = $"Bitte bestätige den folgenden Link um dein Geschäft freizuschalten. Du kannst dich anschließend mit dem Passwort {pwd} einloggen";
                er.ApproveLink = $"{Constants.WebAppBaseUrl}Activate/?guid={shop.ActivationCode}";

                List<string> eMails2Inform;
                if (Constants.CurrentAppSettings.AppConfigurationConstants == 0) //master
                {
                    eMails2Inform = new List<string>
                                    {
                                        exShopRegistration.EMail
                                    }; //ToDo: Wenn die Settings Tabelle existiert dann über die Settings Tabelle
                }
                else
                {
                    eMails2Inform = new List<string>
                                    {
                                        "biss@fotec.at",
                                        exShopRegistration.EMail
                                    }; //ToDo: Wenn die Settings Tabelle existiert dann über die Settings Tabelle
                }

                string email = _mailgenerator.GetRegistrationEmail(er);


                var res = await bm.SendHtmlEMail(Constants.SendEMailAs, eMails2Inform, "Danke für die Registrierung", email, Constants.SendEMailAsDisplayName);
                return new ExSaveDataResult();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="exShopRegistration"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("api/ForgotPasswordShop")]
        public async Task<ExSaveDataResult> ForgotPasswordShop(ExShopForgotPassword exShopRegistration)
        {
            using (Db db = new Db())
            {
                var shop = await db.TblStores.FirstOrDefaultAsync(a => a.EMail.ToLower() == exShopRegistration.EMail.ToLower());

                if (shop == null)
                {
                    return new ExSaveDataResult {Result = EnumSaveDataResult.Error};
                }

                if (exShopRegistration.Step == EnumShopForgotPassword.Step1)
                {
                    //LINK VERSENDEN zum Ändern des Passwortes

                    BissEMail bm = new BissEMail(WebAppSettings.EmailCredentials);

                    ExEmailResetPassword er = new ExEmailResetPassword();

                    er.Message = "Bitte bestätige den folgenden Link, damit dir ein neues Passwort zugesendet wird.";
                    er.ApproveLink = $"{Constants.WebAppBaseUrl}ShopForgotPassword/?guid={shop.ActivationCode}";

                    string email = _mailgenerator.GetPasswordResetEmail(er);

                    List<string> eMails2Inform;
                    if (Constants.CurrentAppSettings.AppConfigurationConstants == 0) //master
                    {
                        eMails2Inform = new List<string>
                                        {
                                            exShopRegistration.EMail
                                        }; //ToDo: Wenn die Settings Tabelle existiert dann über die Settings Tabelle
                    }
                    else
                    {
                        eMails2Inform = new List<string>
                                        {
                                            "biss@fotec.at",
                                            exShopRegistration.EMail
                                        }; //ToDo: Wenn die Settings Tabelle existiert dann über die Settings Tabelle
                    }

                    var res = await bm.SendHtmlEMail(Constants.SendEMailAs, eMails2Inform, "Passwort vergessen?", email, Constants.SendEMailAsDisplayName);
                }
                else if (exShopRegistration.Step == EnumShopForgotPassword.Step2)
                {
                    //PASSWORT VERSENDEN

                    string pwd = PasswordHelper.GeneratePassword(6);

                    shop.Password = PasswordHelper.CumputeHash(pwd);
                    shop.ActivationCode = PasswordHelper.GeneratePassword(10);
                    await db.SaveChangesAsync();

                    BissEMail bm = new BissEMail(WebAppSettings.EmailCredentials);

                    ExEmailNewPassword er = new ExEmailNewPassword();

                    er.Message = "Du kannst dich jetzt mit dem folgenden Passwort einloggen: ";
                    er.NewPassword = pwd;
                    er.ApproveLink = $"{Constants.WebAppBaseUrl}Activate/?guid={shop.ActivationCode}";

                    List<string> eMails2Inform;
                    if (Constants.CurrentAppSettings.AppConfigurationConstants == 0) //master
                    {
                        eMails2Inform = new List<string>
                                        {
                                            exShopRegistration.EMail
                                        };
                    }
                    else
                    {
                        eMails2Inform = new List<string>
                                        {
                                            "biss@fotec.at",
                                            exShopRegistration.EMail
                                        }; //ToDo: Wenn die Settings Tabelle existiert dann über die Settings Tabelle
                    }

                    string email = _mailgenerator.GetNewPasswordEmail(er);

                    var res = await bm.SendHtmlEMail(Constants.SendEMailAs, eMails2Inform, "Passwort wurde geändert", email, Constants.SendEMailAsDisplayName);
                }

                return new ExSaveDataResult();
            }
        }
    }
}