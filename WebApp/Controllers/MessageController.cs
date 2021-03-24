// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange;
using Exchange.Model.Admin;
using Exchange.Notifications;
using Exchange.ServiceAccess;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MessageController : Controller
    {
        /// <summary>
        ///     View für das Senden der Pushnachricht
        /// </summary>
        /// <param name="id">ID des Gerätes</param>
        /// <returns>View</returns>
        public IActionResult Send(int id)
        {
            //Model erzeugen und ID anlegen
            ExPushNotificationData model = new ExPushNotificationData {DeviceId = id};

            return View(model);
        }

        /// <summary>
        ///     Pushnachricht versenden
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Send(ExPushNotificationData notification)
        {
            //PushNachricht versenden
            RestAccess ra = new RestAccess(Constants.ServiceClientEndPointWithApiPrefix);
            await ra.NotificationSendToDevice(notification);
            return RedirectToAction("Index", "Device");
        }

        /// <summary>
        ///     View für das Senden der Pushnachricht
        /// </summary>
        /// <param name="id">ID des Gerätes</param>
        /// <returns>View</returns>
        public IActionResult SendSignalR(int id)
        {
            //Model erzeugen und ID anlegen
            ExSignalRNotificationData model = new ExSignalRNotificationData {UserId = id};

            return View(model);
        }

        /// <summary>
        ///     Pushnachricht versenden
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SendSignalR(ExSignalRNotificationData notification)
        {
            //PushNachricht versenden
            RestAccess ra = new RestAccess(Constants.ServiceClientEndPointWithApiPrefix);

            await ra.SignalRSendMessage(notification);
            return RedirectToAction("Index", "Device");
        }
    }
}