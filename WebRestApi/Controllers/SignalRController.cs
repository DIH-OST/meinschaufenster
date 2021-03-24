// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange.Model;
using Exchange.Notifications;
using WebRestApi.Hubs;
using WebRestApi.Models;

namespace WebRestApi.Controllers
{
    /// <summary>
    ///     <para>SignalRController - API für Kommunikation mit SignalRProxy</para>
    ///     Klasse SignalRController (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class SignalRController : Controller
    {
        /// <summary>
        ///     Dict für Clients
        ///     Pro UserId eine Liste mit verbundenen SignalRClients
        /// </summary>
        public static ConcurrentDictionary<long, List<SignalRClient>> Clients = new ConcurrentDictionary<long, List<SignalRClient>>();

        /// <summary>
        ///     Konstruktor
        /// </summary>
        /// <param name="hubcontext"></param>
        public SignalRController(IHubContext<BissHub> hubcontext)
        {
            HubContext = hubcontext;
        }

        #region Properties

        /// <summary>
        ///     Hub Context
        /// </summary>
        private IHubContext<BissHub> HubContext { get; }

        #endregion

        /// <summary>
        ///     Ein Client hat Daten gesendet
        /// </summary>
        /// <param name="data">Clientdaten</param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("api/SignalRSetClientData/")]
        [HttpPost]
        public bool SignalRSetClientData([FromBody] ExSignalRClientData data)
        {
            //TODO: MKo ... 
            return true;
        }

        /// <summary>
        ///     Notifizierung an einen oder alle verbundenen Clients senden
        /// </summary>
        /// <param name="notification">Notifizierung</param>
        /// <returns>True wenn der Client noch verbunden ansonsten falsch </returns>
        [AllowAnonymous]
        [Route("api/SignalRSendMessage/")]
        [HttpPost]
        public async Task<bool> SignalRSendMessage([FromBody] ExSignalRNotificationData notification)
        {
            //TODO: SignalRMessage versenden!

            ExNotificationData n = new ExNotificationData
                                   {
                                       Body = notification.Body,
                                       CustomData = notification.CustomData,
                                       Title = notification.Title
                                   };

            if (notification.UserId <= 0) //Nachricht geht an alle Benutzer
            {
                foreach (var user in Clients)
                {
                    foreach (var client in user.Value)
                    {
                        await HubContext.Clients.Client(client.ConnectionId).SendAsync("message", n);
                    }
                }
            }
            else //Benutzer im Dict suchen und alle verbundenen Clients benachrichtigen
            {
                if (Clients.ContainsKey(notification.UserId))
                {
                    var clients = Clients[notification.UserId];

                    foreach (var client in clients)
                    {
                        await HubContext.Clients.Client(client.ConnectionId).SendAsync("message", n);
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Verbundene SignalR Clients abfragen
        /// </summary>
        /// <returns>Liste der verbundenen Clients</returns>
        [AllowAnonymous]
        [Route("api/SignalRListConnectedClients/")]
        [HttpGet]
        public List<List<SignalRClient>> SignalRListConnectedClients()
        {
            return Clients.Values.ToList();
        }
    }
}