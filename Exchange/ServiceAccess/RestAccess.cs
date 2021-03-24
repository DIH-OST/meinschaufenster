// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange.Model;
using Exchange.Model.Admin;
using Exchange.Notifications;
using Exchange.PostRequests;

namespace Exchange.ServiceAccess
{
    /// <summary>
    ///     <para>Service Access - Wrapper für Rest Zugriffe</para>
    ///     Klasse RestAccess. (C) 2019 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class RestAccess : RestAccessBase
    {
        /// <summary>
        ///     Funktionen für den Zugriff auf die REST Schnitstelle - projektabhängig
        /// </summary>
        public RestAccess(string serviceClientEndPointWithApiPrefix) : base(serviceClientEndPointWithApiPrefix)
        {
        }

        /// <summary>
        ///     Funktionen für den Zugriff auf die REST Schnitstelle - projektabhängig
        /// </summary>
        /// <param name="secureTokenUserId">User Id für Authentifzierung</param>
        /// <param name="secureTokenUserGuid">Token für Authentifzierung</param>
        /// <param name="serviceClientEndPointWithApiPrefix">Endpunkt</param>
        public RestAccess(string secureTokenUserId, string secureTokenUserGuid, string serviceClientEndPointWithApiPrefix) : base(secureTokenUserId, secureTokenUserGuid, serviceClientEndPointWithApiPrefix)
        {
        }

        #region Notification

        /// <summary>
        ///     Push Notifizierung senden an alle Apps aller User senden
        /// </summary>
        /// <param name="data">Daten</param>
        /// <returns>Id von Mobile Center oder String.Empty wenn es nicht funktioniert hat (IOS, Android, UWP)</returns>
        public async Task<ResultData<bool?>> NotificationSendBroadcast(ExPostNotofication data)
        {
            return await _wap.Post<bool?>("NotificationSendBroadcast", data);
        }

        #endregion

        #region SignalR

        /// <summary>
        ///     Notifizierung an einen oder alle verbundenen Clients senden
        /// </summary>
        /// <param name="notification">Notifizierung</param>
        /// <returns>True wenn der Client noch verbunden ansonsten falsch </returns>
        public async Task<ResultData<bool?>> SignalRSendMessage(ExSignalRNotificationData notification)
        {
            return await _wap.Post<bool?>("SignalRSendMessage", notification);
        }

        #endregion

        #region DeviceController

        /// <summary>
        ///     Alle Geräte abfragen
        /// </summary>
        /// <returns>Liste der Geräte</returns>
        public async Task<ResultData<List<ExExtendedUserDeviceInfo>>> DeviceAllWithUser()
        {
            return await _wap.Get<List<ExExtendedUserDeviceInfo>>("DeviceAllWithUser");
        }

        public async Task<ResultData<ExSaveDataResult>> NotificationSendToDevice(ExPushNotificationData data)
        {
            return await _wap.Post<ExSaveDataResult>("NotificationSendToDevice", data);
        }

        #endregion

        #region Info BissApps

        public async Task<ResultData<string>> Information()
        {
            return await _wap.Get<string>("Information");
        }

        public async Task<ResultData<ExPingResult>> Ping()
        {
            return await _wap.Get<ExPingResult>("Ping");
        }

        #endregion

        #region MeetingController

        /// <summary>
        ///     Default Text abfragen
        /// </summary>
        /// <param name="staffId"></param>
        /// <returns></returns>
        public async Task<ResultData<ExMeetingDefault>> GetDefaultText(int staffId)
        {
            return await _wap.Get<ExMeetingDefault>("GetDefaultText", new List<string> {staffId.ToString()});
        }

        /// <summary>
        ///     Termine eines Shops holen
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="date"></param>
        /// <param name="showTakenSlots"></param>
        /// <returns></returns>
        public async Task<ResultData<List<ExMeeting>>> GetMeetingsForDate(int locationId, DateTime date, bool showTakenSlots = false)
        {
            var request = new ExGetMeetingsForDateRequest
                          {
                              Date = date,
                              LocationId = locationId,
                              ShowTakenSlots = showTakenSlots,
                          };

            return await _wap.Post<List<ExMeeting>>("GetMeetingsForDate", request);
        }

        /// <summary>
        ///     Termine eines Shops holen
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<ResultData<List<ExMeeting>>> GetMyMeetingsForDate(int userId, DateTime date)
        {
            var request = new ExGetMeetingsForDateRequest
                          {
                              Date = date,
                              UserId = userId,
                              ShowTakenSlots = true
                          };

            return await _wap.Post<List<ExMeeting>>("GetMyMeetingsForDate", request);
        }

        /// <summary>
        ///     Termine eines Users holen
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultData<List<ExMeeting>>> GetMyMeetingsForDateWeb(ExGetMeetingsForDateRequest request)
        {
            return await _wap.Post<List<ExMeeting>>("GetMyMeetingsForDateWeb", request);
        }

        /// <summary>
        ///     Termin vereinbaren
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultData<ExSaveDataResult<ExMeeting>>> SetMeeting(ExSaveMeetingRequest request)
        {
            return await _wap.Post<ExSaveDataResult<ExMeeting>>("SetMeeting", request);
        }

        /// <summary>
        ///     Termin vereinbaren
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultData<ExSaveDataResult<ExMeeting>>> SetMeetingWeb(ExSaveMeetingRequest request)
        {
            return await _wap.Post<ExSaveDataResult<ExMeeting>>("SetMeetingWeb", request);
        }

        /// <summary>
        ///     Termin entfernen für Apps
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultData<ExSaveDataResult>> DeleteMeeting(ExRemoveMeetingRequest request)
        {
            return await _wap.Post<ExSaveDataResult>("DeleteMeeting", request);
        }

        /// <summary>
        ///     Termin entfernen für WEbApp
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultData<ExSaveDataResult>> DeleteMeetingWeb(ExRemoveMeetingRequest request)
        {
            return await _wap.Post<ExSaveDataResult>("DeleteMeetingWeb", request);
        }

        #endregion

        #region Public

        /// <summary>
        ///     Infos für die Apps holen
        /// </summary>
        /// <returns></returns>
        public async Task<ResultData<ExAppInfo>> GetAppInfo()
        {
            var result = await _wap.Get<ExAppInfo>("GetAppInfo");

            if (result.Result == null)
                return new ResultData<ExAppInfo>
                       {
                           Result = new ExAppInfo
                                    {
                                        EMail = "info@meinschaufenster.at",
                                        UrlToEula = $"{Constants.WebAppBaseUrl}assets/AGB_Datenschutz.pdf",
                                        UrlToInfoMaterial = $"{Constants.WebAppBaseUrl}assets/Info_Kunden.pdf",
                                        UrlToMerchantVideo = "https://www.youtube.com/watch?v=eEqPVhu3IhI",
                                        UrlToCustomerVideo = "https://www.youtube.com/watch?v=Y9Bh93UPlFU"
                                    },
                           Status = ResultTypes.Ok
                       };

            return result;
        }

        public async Task<ResultData<string>> GetMaintenanceInfo()
        {
            var result = await _wap.Get<string>("GetMaintenanceInfo");
            return result;
        }

        #endregion

        #region ShopController

        /// <summary>
        ///     Shop löschen
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultData<bool>> DeleteShop(ExDeleteRequest request)
        {
            return await _wap.Post<bool>("DeleteShop", request);
        }

        /// <summary>
        ///     Listen für Filter laden
        /// </summary>
        /// <returns></returns>
        public async Task<ResultData<ExFilterInfos>> GetFilterInfos()
        {
            return await _wap.Get<ExFilterInfos>("GetFilterInfos");
        }

        /// <summary>
        ///     Alle Shops für einen Request holen
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultData<List<ExShopShort>>> GetShops(ExGetShopsRequest request)
        {
            return await _wap.Post<List<ExShopShort>>("GetShops", request);
        }

        /// <summary>
        ///     Shopinfos holen
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public async Task<ResultData<ExShop>> GetShopInfo(int locationId)
        {
            return await _wap.Get<ExShop>("GetShopInfo", new List<string> {locationId.ToString()});
        }

        /// <summary>
        ///     Registrierung eines Shops
        /// </summary>
        /// <param name="shop">Shopdaten</param>
        /// <returns></returns>
        public async Task<ResultData<ExSaveDataResult>> RegisterShop(ExShopRegistration shop)
        {
            return await _wap.Post<ExSaveDataResult>("RegisterShop", shop);
        }

        /// <summary>
        ///     Passwort vergessen eines Shops
        /// </summary>
        /// <param name="shop">Shopdaten</param>
        /// <returns></returns>
        public async Task<ResultData<ExSaveDataResult>> ForgotPasswordShop(ExShopForgotPassword shop)
        {
            return await _wap.Post<ExSaveDataResult>("ForgotPasswordShop", shop);
        }

        #endregion

        #region UserController

        /// <summary>
        ///     Benutzerstammdaten ändern
        /// </summary>
        /// <param name="user">Benutzerdaten</param>
        /// <returns></returns>
        public async Task<ResultData<ExSaveDataResult>> UserUpdate(ExUserAccountData user)
        {
            return await _wap.Post<ExSaveDataResult>("UserUpdate", user);
        }

        /// <summary>
        ///     Passwort ändern
        /// </summary>
        /// <param name="password">Altes und neues Passwort</param>
        /// <returns></returns>
        public async Task<ResultData<ExSaveDataResult>> UserUpdatePassword(ExPostUserChangePasswortData password)
        {
            return await _wap.Post<ExSaveDataResult>("UserUpdatePassword", password);
        }

        /// <summary>
        ///     Ein Gerät eines User anlegen bzw. aktualisieren
        /// </summary>
        /// <param name="userDeviceData"></param>
        /// <returns></returns>
        public async Task<ResultData<bool?>> UserDeviceUpdate(ExUserDeviceInfo userDeviceData)
        {
            return await _wap.Post<bool?>("UserDeviceUpdate", userDeviceData);
        }

        /// <summary>
        ///     Ein Gerät eines Benutzers aus dessen Geräteliste entfernen für Apps
        /// </summary>
        /// <param name="userDeviceData"></param>
        /// <returns></returns>
        public async Task<ResultData<bool?>> UserDeviceDelete(ExPostUserDeviceDelete userDeviceData)
        {
            return await _wap.Post<bool?>("UserDeviceDelete", userDeviceData);
        }

        /// <summary>
        ///     Ein Gerät eines Benutzers aus dessen Geräteliste entfernen für WebApp
        /// </summary>
        /// <param name="userDeviceData"></param>
        /// <returns></returns>
        public async Task<ResultData<bool?>> UserDeviceDeleteWeb(ExPostUserDeviceDelete userDeviceData)
        {
            return await _wap.Post<bool?>("UserDeviceDeleteWeb", userDeviceData);
        }

        /// <summary>
        ///     1. Schritt der Anmeldung => UserName (EMail) wird empfangen. Wenn User existiert wird Id zurück gegeben.
        ///     Wenn neuer User dann IsNewUser = true und senden SMS mit Passwort.
        ///     Es wird auch mitgegeben ob der Account gerade gesperrt ist.
        /// </summary>
        /// <param name="userPhone">User E-Mail (Login Name)</param>
        /// <returns> Passwort Hash oder String.Empty</returns>
        public async Task<ResultData<ExCheckUser>> UserCheck(string userPhone)
        {
            return await _wap.Get<ExCheckUser>("UserCheck", new List<string> {userPhone});
        }

        /// <summary>
        ///     Demo User Id abfragen (wenn Demo User verfügbar)
        /// </summary>
        /// <returns></returns>
        public async Task<ResultData<int?>> UserDemoId()
        {
            return await _wap.Get<int?>("UserDemoId");
        }

        /// <summary>
        ///     2. Schritt Anmeldung => Passwort lokal gehashed und wird nun überprüft.
        /// </summary>
        /// <returns>Userdaten oder null wenn nicht verfügbar oder Passwort</returns>
        public async Task<ResultData<ExUserAccountDataResult>> UserAccountData(ExPostUserPasswortData userData)
        {
            return await _wap.Post<ExUserAccountDataResult>("UserAccountData", userData);
        }

        /// <summary>
        ///     Passwort zurücksetzen - Email mit Link zum zurücksetzen senden
        /// </summary>
        /// <param name="userId">Id des User aus TableUser</param>
        /// <returns>Ja - true</returns>
        public async Task<ResultData<bool?>> UserStartResetPassword(int userId)
        {
            return await _wap.Get<bool?>("UserStartResetPassword", new List<string> {userId.ToString()});
        }

        /// <summary>
        ///     Userdaten löschen für Apps
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ResultData<ExSaveDataResult>> DeleteUser(int userId)
        {
            return await _wap.Post<ExSaveDataResult>("DeleteUser", new ExDeleteRequest {Id = userId,});
        }

        /// <summary>
        ///     Userdaten löschen für WebApp
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultData<ExSaveDataResult>> DeleteUserWeb(ExDeleteRequest request)
        {
            return await _wap.Post<ExSaveDataResult>("DeleteUserWeb", request);
        }

        #endregion
    }
}