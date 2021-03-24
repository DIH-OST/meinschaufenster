// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Notifications
{
    /// <summary>
    ///     <para>Senden von Notifizierungen</para>
    ///     Klasse MobileCenterNotification. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class MobileCenterNotification
    {
        private readonly MobileCenterNotificationConfiguration _configuration;

        /// <summary>
        ///     Senden von Notifizierungen
        /// </summary>
        /// <param name="configuration">Configuration</param>
        public MobileCenterNotification(MobileCenterNotificationConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        ///     Push Notifizierung senden
        /// </summary>
        /// <param name="campaignName">Name intern für Azure Mobile Center</param>
        /// <param name="body">Inhalt der Notifizierung</param>
        /// <param name="title">Titel (optional)</param>
        /// <param name="plattform">Plattform an die gesendet wird</param>
        /// <param name="data">Daten (optional)</param>
        /// <param name="devices">Geräte an die gesendet wird (wenn null => alle Geräte)</param>
        /// <returns>Id von Mobile Center oder String.Empty wenn es nicht funktioniert hat</returns>
        public async Task<string> Send(string campaignName, string body, EnumPlattform plattform, string title = "", Dictionary<string, string> data = null, List<string> devices = null)
        {
            string appName = "";
            switch (plattform)
            {
                case EnumPlattform.XamarinIos:
                    appName = _configuration.AppNameIOs;
                    break;
                case EnumPlattform.XamarinAndroid:
                    appName = _configuration.AppNameAndroid;
                    break;
                case EnumPlattform.XamarinUwp:
                    appName = _configuration.AppNameUwp;
                    break;
                case EnumPlattform.Wpf:
                case EnumPlattform.Web:
                    throw new NotSupportedException();
                default:
                    throw new ArgumentOutOfRangeException(nameof(plattform), plattform, null);
            }

            if (String.IsNullOrEmpty(appName))
                return String.Empty;

            if (String.IsNullOrEmpty(campaignName))
                campaignName = appName;

            //Service und url
            HttpClient mobileService = new HttpClient(new HttpClientHandler());
            mobileService.DefaultRequestHeaders.Add("X-API-Token", _configuration.Token);

            //https://api.mobile.azure.com/v0.1/apps/FOTEC-Biss/Biss-mvvm/push/notifications
            string pushUrl = $"{_configuration.BaseApiUrl}apps/{_configuration.OrganizationName}/{appName}/push/notifications";

            if (!String.IsNullOrEmpty(title))
            {
                title = string.Concat(title.Take(100));
            }

            if (!String.IsNullOrEmpty(body))
            {
                body = string.Concat(body.Take(100));
            }

            // Notwendig damit unter iOS auch ein Benachrichtigungston abgespielt wird.
            // https://docs.microsoft.com/en-us/appcenter/sdk/push/ios <- reserved keywords on iOS platform
            if (plattform == EnumPlattform.XamarinIos)
            {
                if (data == null)
                {
                    data = new Dictionary<string, string>();
                    data.Add("sound", "biss");
                }
                else
                {
                    data.Add("sound", "biss");
                }
            }

            // Reserved iOS:
            data.Add("badge", "-");
            data.Add("sound", "-");
            data.Add("content-available", "-");
            data.Add("mutable-content", "-");

            // Reserved Droid:
            data.Add("color", "-");
            data.Add("icon", "-");
            data.Add("sound", "-");

            // Reserved UWP:
            data.Add("audio", "-");
            data.Add("image", "-");

            //Nachrichten Objekt erzeugen
            var notificationContent = new NotificationContent
                                      {
                                          Name = campaignName,
                                          Title = title,
                                          Body = body,
                                          CustomData = data
                                      };


            //Nachrichten mit Target erzeugen
            var requests = new List<MobileCenterNotificationData>();
            if (devices != null && devices.Any())
            {
                const int maxDevicesPerNotification = 20;

                for (int i = 0; i < devices.Count; i += maxDevicesPerNotification)
                {
                    var requestBody = new MobileCenterNotificationData();
                    var deviceCountForThisNotification = (i + maxDevicesPerNotification) <= devices.Count ? maxDevicesPerNotification : devices.Count - i;
                    requestBody.NotificationContent = notificationContent;
                    requestBody.NotificationTarget = new NotificationTargetDeviceList
                                                     {
                                                         Devices = devices.GetRange(i, deviceCountForThisNotification)
                                                     };
                    requests.Add(requestBody);
                }
            }
            else
            {
                var requestBody = new MobileCenterNotificationData
                                  {
                                      NotificationContent = notificationContent,
                                      NotificationTarget = null
                                  };
                requests.Add(requestBody);
            }

            object lastNotificationResponse = null;

            //Notification versenden
            foreach (var request in requests)
            {
                //Serialisieren und Nachrichten verschicken
                var abc = JsonConvert.SerializeObject(request);
                var sc = new StringContent(abc, Encoding.UTF8, "text/json");
                var response = await mobileService.PostAsync(pushUrl, sc);

                Logging.Log.LogInfo($"Notification sende: {abc}");
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Logging.Log.LogError("Notification could not be sent, access denied!");
                    break;
                }

                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    lastNotificationResponse = JsonConvert.DeserializeObject<NotificationResponseAccepted>(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    var error = JsonConvert.DeserializeObject<NotificationResponseFailure>(await response.Content.ReadAsStringAsync());
                    lastNotificationResponse = error;
                    Logging.Log.LogError($"Notification could not be sent:  Code:{error.Error.Code}  Message:{error.Error.Message} ");
                    break;
                }
            }

            //result
            var successNotification = lastNotificationResponse as NotificationResponseAccepted;
            if (successNotification != null)
                return successNotification.NotificationId;
            return String.Empty;
        }

        /// <summary>
        ///     Push Notifizierung senden an alle Apps aller User senden
        /// </summary>
        /// <param name="campaignName">Name intern für Azure Mobile Center</param>
        /// <param name="body">Inhalt der Notifizierung</param>
        /// <param name="title">Titel (optional)</param>
        /// <param name="data">Daten (optional)</param>
        /// <returns>Id von Mobile Center oder String.Empty wenn es nicht funktioniert hat (IOS, Android, UWP)</returns>
        public async Task<List<string>> SendBroadcast(string campaignName, string body, string title = "", Dictionary<string, string> data = null)
        {
            var iosData = new Dictionary<string, string>();

            if (data != null)
            {
                iosData = data;
            }

            iosData.Add("sound", "default");

            var result = new List<string>
                         {
                             await Send(campaignName, body, EnumPlattform.XamarinIos, title, iosData),
                             await Send(campaignName, body, EnumPlattform.XamarinAndroid, title, data),
                             await Send(campaignName, body, EnumPlattform.XamarinUwp, title, data)
                         };


            return result;
        }

        /// <summary>
        ///     Gerät vom MobileCenter löschen
        /// </summary>
        /// <param name="deviceToken">DeviceToken</param>
        /// <param name="plattform">Plattform</param>
        /// <returns></returns>
        public async Task<bool> RemoveDevice(string deviceToken, EnumPlattform plattform)
        {
            string appName = "";
            switch (plattform)
            {
                case EnumPlattform.XamarinIos:
                    appName = _configuration.AppNameIOs;
                    break;
                case EnumPlattform.XamarinAndroid:
                    appName = _configuration.AppNameAndroid;
                    break;
                case EnumPlattform.XamarinUwp:
                    appName = _configuration.AppNameUwp;
                    break;
                case EnumPlattform.Wpf:
                case EnumPlattform.Web:
                    throw new NotSupportedException();
                default:
                    throw new ArgumentOutOfRangeException(nameof(plattform), plattform, null);
            }

            if (String.IsNullOrEmpty(appName))
                return false;

            //Service und url
            HttpClient mobileService = new HttpClient(new HttpClientHandler());
            mobileService.DefaultRequestHeaders.Add("X-API-Token", _configuration.Token);

            //https://api.mobile.azure.com/v0.1/apps/FOTEC-Biss/Biss-mvvm/push/notifications
            string url = $"{_configuration.BaseApiUrl}apps/{_configuration.OrganizationName}/{appName}/push/devices/{deviceToken}";

            var result = await mobileService.DeleteAsync(url);
            return result.IsSuccessStatusCode;
        }


        public async Task<bool> GetAudience(EnumPlattform plattform, string group)
        {
            try
            {
                string appName = "";
                switch (plattform)
                {
                    case EnumPlattform.XamarinIos:
                        appName = _configuration.AppNameIOs;
                        break;
                    case EnumPlattform.XamarinAndroid:
                        appName = _configuration.AppNameAndroid;
                        break;
                    case EnumPlattform.XamarinUwp:
                        appName = _configuration.AppNameUwp;
                        break;
                    case EnumPlattform.Wpf:
                    case EnumPlattform.Web:
                        throw new NotSupportedException();
                    default:
                        throw new ArgumentOutOfRangeException(nameof(plattform), plattform, null);
                }

                if (String.IsNullOrEmpty(appName))
                    return false;

                //Service und url
                HttpClient mobileService = new HttpClient(new HttpClientHandler());
                mobileService.DefaultRequestHeaders.Add("X-API-Token", _configuration.Token);

                //https://api.appcenter.ms/v0.1/apps/FOTEC-BISS/biss.demo.dev-Android/analytics/audiences/Testgruppe
                string url = $"{_configuration.BaseApiUrl}apps/{_configuration.OrganizationName}/{appName}/analytics/audiences/{group}";

                var result = await mobileService.GetAsync(url);

                if (result.IsSuccessStatusCode)
                {
                    var stringRes = await result.Content.ReadAsStringAsync();

                    Logging.Log.LogInfo($"Audience: {stringRes}");

                    return !string.IsNullOrWhiteSpace(stringRes);
                }

                return false;
            }
            catch (Exception e)
            {
                Logging.Log.LogError($"Test: {e}");
                return false;
            }
        }
    }
}