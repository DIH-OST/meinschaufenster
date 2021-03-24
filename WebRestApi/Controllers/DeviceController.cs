// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;
using Exchange.Model;

namespace WebRestApi.Controllers
{
    /// <summary>
    ///     Geräte-Controller
    /// </summary>
    [ApiController]
    //[Authorize]
    [AllowAnonymous]
    public class DeviceController : Controller
    {
        /// <summary>
        ///     Alle Geräte eines bestimmten Benutzers
        /// </summary>
        /// <returns>Liste der Geräte</returns>
        [Route("api/DeviceAllWithUser/")]
        [HttpGet]
        public async Task<List<ExExtendedUserDeviceInfo>> DeviceAllWithUser()
        {
            Logging.Log.LogInfo("AllDevicesWithUser");

            using (var db = new Db())
            {
                var result = new List<ExExtendedUserDeviceInfo>();

                result = await db.TblUserDevices.Include(tu => tu.TblUser).Select(s => new ExExtendedUserDeviceInfo
                                                                                       {
                                                                                           UserId = s.TblUserId,
                                                                                           UserName = s.TblUser.PhoneNumber,
                                                                                           DeviceId = s.Id,
                                                                                           DeviceIdiom = s.Device.DeviceIdiom,
                                                                                           DeviceToken = s.Device.DeviceToken,
                                                                                           DeviceType = s.Device.DeviceType,
                                                                                           IsAppRunning = s.Device.IsAppRunning,
                                                                                           LastDateTimeUtcOnline = s.Device.LastDateTimeUtcOnline,
                                                                                           OperatingSystemVersion = s.Device.OperatingSystemVersion,
                                                                                           Plattform = s.Device.Plattform
                                                                                       }).ToListAsync();

                return result;
            }
        }
    }
}