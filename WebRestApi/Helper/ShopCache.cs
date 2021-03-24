// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;
using Exchange;
using Exchange.Model;
using Exchange.PostRequests;

namespace WebRestApi.Helper
{
    /// <summary>
    ///     <para>Einfacher Cache für die Geschäfte</para>
    ///     Klasse ShopCache. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class ShopCache
    {
        private static List<ExShopShort> _shops;
        private static readonly TimeSpan _timeToReload = new TimeSpan(0, 0, 0, 30);
        private static DateTime _nextTimeToLoad;
        private static bool _reloading;


        /// <summary>
        ///     Einfacher schneller Cache für die Geschäfte
        /// </summary>
        /// <param name="request"></param>
        /// <param name="namofGlyphToIcon"></param>
        /// <returns></returns>
        public static List<ExShopShort> GetShops(ExGetShopsRequest request, string namofGlyphToIcon)
        {
            if (_shops == null)
            {
                var tmp = LoadShopsFromDb(request, namofGlyphToIcon);
                _shops = tmp;
                _nextTimeToLoad = DateTime.Now + _timeToReload;
            }
            else if (!_reloading && DateTime.Now > _nextTimeToLoad)
            {
                _reloading = true;
                Task.Run(() =>
                {
                    var tmp = LoadShopsFromDb(request, namofGlyphToIcon);
                    _shops = tmp;
                    _nextTimeToLoad = DateTime.Now + _timeToReload;
                    _reloading = false;
                });
            }

            return _shops;
        }

        private static List<ExShopShort> LoadShopsFromDb(ExGetShopsRequest request, string namofGlyphToIcon)
        {
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

                    var iconDroid = Constants.ServiceClientEndPointWithApiPrefix + namofGlyphToIcon + $"/{shop?.MainCategory?.Glyph}" +
                                    $"/{(shop.IsOpen ? Color.ForestGreen : Color.Crimson).ToArgb()}" +
                                    $"/{Color.White.ToArgb()}" +
                                    $"/{Color.Transparent.ToArgb()}" +
                                    "/128" +
                                    $"/{string.IsNullOrWhiteSpace(shop?.MainCategory?.Glyph)}";

                    var iconIos = Constants.ServiceClientEndPointWithApiPrefix + namofGlyphToIcon + $"/{shop?.MainCategory?.Glyph}" +
                                  $"/{(shop.IsOpen ? Color.ForestGreen : Color.Crimson).ToArgb()}" +
                                  $"/{Color.White.ToArgb()}" +
                                  $"/{Color.Transparent.ToArgb()}" +
                                  "/50" +
                                  $"/{string.IsNullOrWhiteSpace(shop?.MainCategory?.Glyph)}";

                    shop.MainCategory.Pin = new BissPinInfo(shop.Position, shop.Name, iconDroid, iconIos);

                    #endregion

                    res.Add(shop);
                }


                //ShopCheck
                res.RemoveAll(s => String.IsNullOrEmpty(s.Name) || s.Position == null || s.Position.Latitude == 0 || s.Position.Longitude == 0);

                //foreach (var s in res)
                //{
                //    if (String.IsNullOrEmpty(s.Name) || s.Position == null || s.Position.Latitude == 0 || s.Position.Longitude == 0)
                //    {
                //        res.Remove(s);
                //    }
                //}


                Logging.Log.LogWarning("Finished " + sw.Elapsed);
                sw.Stop();

                return res;
            }
        }
    }
}