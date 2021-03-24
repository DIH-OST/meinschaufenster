// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;
using WebRestApi.Controllers;
using WebRestApi.Models;

namespace WebRestApi.Hubs
{
    /// <summary>
    ///     <para>Biss Hub</para>
    ///     Klasse BissHub. (C) 2018 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class BissHub : Hub
    {
        /// <summary>
        ///     Called when a new connection is established with the hub.
        /// </summary>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous connect.</returns>
        public override async Task OnConnectedAsync()
        {
            return;
            await base.OnConnectedAsync();
        }

        /// <summary>
        ///     Client hat wurde getrennt
        /// </summary>
        /// <param name="exception">Fehler</param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            return;

            foreach (var c in SignalRController.Clients.ToList())
            {
                var srClient = c.Value.FirstOrDefault(src => src.ConnectionId == Context.ConnectionId);

                if (srClient != null)
                {
                    using (var db = new Db())
                    {
                        var userDevices = await db.TblUserDevices.Where(u => u.TblUserId == c.Key).Select(s => s.Device).ToListAsync();

                        if (userDevices.Any())
                        {
                            var device = userDevices.FirstOrDefault(s => s.DeviceToken == srClient.DeviceToken);

                            if (device != null)
                            {
                                device.IsAppRunning = false;
                                await db.SaveChangesAsync();
                            }
                        }
                    }

                    SignalRController.Clients[c.Key].Remove(srClient);
                }

                if (SignalRController.Clients[c.Key].Count <= 0)
                {
                    List<SignalRClient> cTmp;
                    SignalRController.Clients.TryRemove(c.Key, out cTmp);
                }
            }
        }

        /// <summary>
        ///     Client loggt sich ein
        /// </summary>
        /// <param name="userId">BenutzerId</param>
        /// <param name="userToken">Benutzertoken</param>
        /// <param name="deviceToken">Gerätetoken</param>
        /// <returns></returns>
        public async Task Login(long userId, string userToken, string deviceToken)
        {
            return;

            //Neuer Benutzer + Neues Device
            if (!SignalRController.Clients.ContainsKey(userId))
            {
                SignalRController.Clients.TryAdd(userId, new List<SignalRClient> {new SignalRClient {ConnectionId = Context.ConnectionId, DeviceToken = deviceToken, UserToken = userToken}});
            }
            else //Benutzer bereits im Dictionary
            {
                var srClients = SignalRController.Clients[userId];
                var srClient = srClients.FirstOrDefault(src => src.DeviceToken == deviceToken);

                //DeviceToken bereits in der Liste -> Aktualisieren
                if (srClient != null)
                {
                    srClient.ConnectionId = Context.ConnectionId;
                }
                else
                {
                    srClients.Add(new SignalRClient {ConnectionId = Context.ConnectionId, DeviceToken = deviceToken, UserToken = userToken});
                }
            }

            using (var db = new Db())
            {
                var userDevices = await db.TblUserDevices.Where(u => u.TblUserId == userId).Select(s => s.Device).ToListAsync();

                if (userDevices.Any())
                {
                    var device = userDevices.FirstOrDefault(s => s.DeviceToken == deviceToken);

                    if (device != null)
                    {
                        device.IsAppRunning = true;
                        await db.SaveChangesAsync();
                    }
                }
            }
        }

        /// <summary>
        ///     Client setzt Daten
        /// </summary>
        /// <param name="key">Schlüssel</param>
        /// <param name="value">Wert</param>
        /// <returns></returns>
        public void SetData(string key, string value)
        {
            //TODO ... Wie auch immer gewünscht!
        }
    }
}