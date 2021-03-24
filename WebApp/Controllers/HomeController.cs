// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange;
using Exchange.Model;
using Exchange.PostRequests;
using Exchange.ServiceAccess;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            try
            {
                var res = new RestAccess(Constants.ServiceClientEndPointWithApiPrefix);

                var result = await res.GetMaintenanceInfo();
                if (result.Ok && !string.IsNullOrEmpty(result.Result))
                {
                    ViewData["maintenance"] = result.Result;
                }
            }
            catch (Exception ex)
            {
            }

            return View("IndexV2");
        }

        public IActionResult Dev()
        {
            return View("IndexV2");
        }

        [HttpGet]
        public async Task<IActionResult> LoadStoreDetail(int storeId)
        {
            RestAccess ra = new RestAccess(Constants.ServiceClientEndPointWithApiPrefix);

            var res = await ra.GetShopInfo(storeId);
            ExShopViewModel vm = new ExShopViewModel();
            vm.Shop = res.Result;

            vm.HoursToday = $"{Convert(vm.Shop.OpeningHours.First())}";
            string all = "";

            foreach (var resultOpeningHour in vm.Shop.OpeningHours)
            {
                all += Convert(resultOpeningHour) + ", ";
            }

            all = all.Substring(0, all.Length - 2);

            vm.AllHours = all;

            return PartialView(vm);
        }

        [HttpGet]
        public async Task<IActionResult> LoadStores()
        {
            RestAccess ra = new RestAccess(Constants.ServiceClientEndPointWithApiPrefix);
            var request = new ExGetShopsRequest
                          {
                              MyPosition = new BissPosition(47, 16),
                              Range = 2500
                          };

            var allShops = await ra.GetShops(request);

            ShopViewModel vm = new ShopViewModel();
            vm.features = new List<Feature>();
            vm.type = "FeatureCollection";

            if (allShops != null && allShops.Ok)
            {
                foreach (var exShop in allShops.Result)
                {
                    Feature f = new Feature();
                    f.geometry = new Geometry();
                    f.geometry.type = "Point";
                    f.geometry.coordinates = new List<double>();
                    f.geometry.coordinates.Add(exShop.Position.Longitude);
                    f.geometry.coordinates.Add(exShop.Position.Latitude);
                    f.type = "Feature";
                    f.properties = new Properties();
                    f.properties.category = String.Join(", ", exShop.Categories.Select(a => a.Name));

                    f.properties.name = exShop.Name;

                    f.properties.storeid = exShop.Id.ToString();

                    f.properties.isopen = exShop.IsOpen;

                    var color = exShop.IsOpen ? "%23FF228B22" : "%23FFDC143C";

                    var glyph = exShop.MainCategory?.Glyph ?? "E994";

                    f.properties.symbol = $"{Constants.ServiceClientEndPointWithApiPrefix}GlyphToIcon/{glyph}/{color}/%23FFFFFFFF/%20%2300FFFFFF/32/false";

                    vm.features.Add(f);
                }
            }

            // Get the data
            string ret = vm.ToJson();

            return Content(ret, "application/json");
        }

        public object Convert(object value)
        {
            if (!(value is ExOpeningHour opening))
            {
                return string.Empty;
            }

            var uiString = string.Empty;

            switch (opening.Day.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    uiString = "Mo: ";
                    break;
                case DayOfWeek.Tuesday:
                    uiString = "Di: ";
                    break;
                case DayOfWeek.Wednesday:
                    uiString = "Mi: ";
                    break;
                case DayOfWeek.Thursday:
                    uiString = "Do: ";
                    break;
                case DayOfWeek.Friday:
                    uiString = "Fr: ";
                    break;
                case DayOfWeek.Saturday:
                    uiString = "Sa: ";
                    break;
                case DayOfWeek.Sunday:
                    uiString = "So: ";
                    break;
            }

            if (!opening.TimeFrom.HasValue || !opening.TimeTo.HasValue)
            {
                uiString += "Geschlossen";
            }
            else
                uiString += $"{opening.TimeFrom.Value.AddHours(2).ToString("HH:mm")} - {opening.TimeTo.Value.AddHours(2).ToString("HH:mm")}";

            return uiString;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}