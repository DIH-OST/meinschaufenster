// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Notifications
{
    /// <summary>
    ///     <para>Daten für Azure Mobile Center Notifications</para>
    ///     Klasse MobileCenterNotificationData. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class MobileCenterNotificationData
    {
        #region Properties

        /// <summary>
        ///     null für broadcast, ansonsten entweder NotificationTargetDeviceList oder NotificationTargetAudience
        /// </summary>
        [JsonProperty(PropertyName = "notification_target")]
        public NotificationTarget NotificationTarget { get; set; }

        /// <summary>
        ///     Inhalt der Benachrichtigung
        /// </summary>
        [JsonProperty(PropertyName = "notification_content")]
        public NotificationContent NotificationContent { get; set; }

        #endregion
    }

    /// <summary>
    ///     Notifizierungsziel base
    /// </summary>
    public abstract class NotificationTarget
    {
        #region Properties

        /// <summary>
        ///     Typ
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public virtual string Type { get; set; }

        #endregion
    }

    /// <summary>
    ///     Nachrichtenziel: Geräteliste
    /// </summary>
    public class NotificationTargetDeviceList : NotificationTarget
    {
        #region Properties

        /// <summary>
        ///     Typ
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public override string Type { get; set; } = "devices_target";

        /// <summary>
        ///     Geräteliste
        /// </summary>
        [JsonProperty(PropertyName = "devices")]
        public List<string> Devices { get; set; } //List of Device Identifiers

        #endregion
    }

    /// <summary>
    ///     Nachrichtenziel: Benutzergruppe
    /// </summary>
    public class NotificationTargetAudience : NotificationTarget
    {
        #region Properties

        /// <summary>
        ///     Typ
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public override string Type { get; set; } = "audiences_target";

        /// <summary>
        ///     Audiences
        /// </summary>
        [JsonProperty(PropertyName = "audiences")]
        public List<string> Audiences { get; set; } //List of Audiences

        #endregion
    }

    /// <summary>
    ///     Nachrichteninhalt
    /// </summary>
    public class NotificationContent
    {
        #region Properties

        /// <summary>
        ///     Name der notification
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        ///     Notification Title
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        /// <summary>
        ///     Notification Body
        /// </summary>
        [JsonProperty(PropertyName = "body")]
        public string Body { get; set; }

        /// <summary>
        ///     Key Value Pairs / Daten
        /// </summary>
        [JsonProperty(PropertyName = "custom_data")]
        public Dictionary<string, string> CustomData { get; set; }

        #endregion
    }

    /// <summary>
    ///     Notifizierung erfolgreich versandt
    /// </summary>
    public class NotificationResponseAccepted
    {
        #region Properties

        /// <summary>
        ///     NotificationId
        /// </summary>
        [JsonProperty(PropertyName = "notification_id")]
        public string NotificationId { get; set; }

        #endregion
    }

    /// <summary>
    ///     Notifizierung konnte nicht versendet werden
    /// </summary>
    public class NotificationResponseFailure
    {
        #region Nested Types

        /// <summary>
        ///     Fehlernachricht
        /// </summary>
        public class NotificationError
        {
            #region Properties

            /// <summary>
            ///     Fehler
            /// </summary>
            [JsonProperty(PropertyName = "code")]
            public string Code { get; set; }

            /// <summary>
            ///     Fehler
            /// </summary>
            [JsonProperty(PropertyName = "message")]
            public string Message { get; set; }

            #endregion
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Fehler
        /// </summary>
        [JsonProperty(PropertyName = "error")]
        public NotificationError Error { get; set; }

        #endregion
    }
}