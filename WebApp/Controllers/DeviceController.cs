// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange;
using Exchange.Model;
using Exchange.PostRequests;
using Exchange.ServiceAccess;
using WebRestApi.Helper;

namespace WebApp.Controllers
{
    /// <summary>
    ///     Geräte-Kontroller
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class DeviceController : Controller
    {
        /// <summary>
        ///     Index
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        ///     Geräte lesen
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IActionResult> Devices_Read([DataSourceRequest] DataSourceRequest request)
        {
            RestAccess ra = new RestAccess(Constants.ServiceClientEndPointWithApiPrefix);

            var da = await ra.DeviceAllWithUser();

            return Json(da.Result.ToDataSourceResult(request));
        }

        /// <summary>
        ///     Ein Gerät löschen
        /// </summary>
        /// <param name="request"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Device_Destroy([DataSourceRequest] DataSourceRequest request, ExExtendedUserDeviceInfo device)
        {
            RestAccess ra = new RestAccess(Constants.ServiceClientEndPointWithApiPrefix);
            ExPostUserDeviceDelete model = new ExPostUserDeviceDelete();
            model.UserId = device.UserId;
            model.DeviceToken = device.DeviceToken;
            model.Plattform = device.Plattform;
            model.CheckPassword = WebAppSettings.CheckPassword;

            var res = await ra.UserDeviceDeleteWeb(model);

            return Json(true);
        }
    }
}