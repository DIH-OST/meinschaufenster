// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;
using Exchange;
using Exchange.Model.Admin;
using Exchange.Notifications;
using Exchange.PostRequests;

namespace WebRestApi.Controllers
{
    /// <summary>
    ///     <para>NotificationController</para>
    ///     Klasse NotificationController. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class NotificationController : Controller
    {
        /// <summary>
        ///     Push Notifizierung senden an alle Apps aller User senden
        /// </summary>
        /// <param name="data">Daten</param>
        /// <returns>Id von Mobile Center oder String.Empty wenn es nicht funktioniert hat (IOS, Android, UWP)</returns>
        [AllowAnonymous]
        [Route("api/NotificationSendBroadcast/")]
        [HttpPost]
        public bool NotificationSendBroadcast([FromBody] ExPostNotofication data)
        {
            Logging.Log.LogInfo($"NotificationSendBroadcast/{data.CampaignName}/{data.Body}/{data.Title}");
            var service = new MobileCenterNotification(Constants.MobileCenterNotification);

            Task.Run(async () => await service.SendBroadcast(data.CampaignName, data.Body, data.Title, data.Data));
            return true;
        }

        /// <summary>
        ///     Notifizierung an ein bestimmtes Gerät per Push versenden
        /// </summary>
        /// <param name="data">Daten</param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("api/NotificationSendToDevice/")]
        [HttpPost]
        public async Task<ExSaveDataResult> NotificationSendToDevice([FromBody] ExPushNotificationData data)
        {
            using (Db db = new Db())
            {
                var userDev = await db.TblUserDevices.FirstOrDefaultAsync(a => a.Id == data.DeviceId);

                if (userDev != null)
                {
                    var service = new MobileCenterNotification(Constants.MobileCenterNotification);
                    await service.Send("", data.Body, userDev.Device.Plattform, data.Title, null, new List<string> {userDev.Device.DeviceToken});
                    return new ExSaveDataResult();
                }

                return new ExSaveDataResult();
            }
        }

        [AllowAnonymous]
        [Route("api/ClearUserDevices/")]
        [HttpGet]
        public async Task<ExSaveDataResult> ClearUserDevices()
        {
            using (var db = new Db())
            {
                var data = await db.TblUserDevices.ToListAsync();
                db.TblUserDevices.RemoveRange(data);

                try
                {
                    var service = new MobileCenterNotification(Constants.MobileCenterNotification);
                    if (data.Any())
                    {
                        foreach (var userDevice in data)
                        {
                            await service.RemoveDevice(userDevice.Device.DeviceToken, userDevice.Device.Plattform);
                        }
                    }

                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Logging.Log.LogWarning($"UserDeviceDelete SaveChanges: {e}");
                    return ExSaveDataResult.GetDefaultSaveError();
                }

                return ExSaveDataResult.GetDefaultSuccess();
            }
        }

        [AllowAnonymous]
        [Route("api/NotificationTestAudience/")]
        [HttpGet]
        public async Task<ExSaveDataResult> NotificationTestAudience()
        {
            Logging.Log.LogInfo("NotificationTestAudience");
            var service = new MobileCenterNotification(Constants.MobileCenterNotification);

            Task.Run(async () => await service.GetAudience(EnumPlattform.XamarinAndroid, "Testgruppe"));
            return ExSaveDataResult.GetDefaultSuccess();
        }
    }
}