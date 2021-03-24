// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Model.Admin
{
    /// <summary>
    ///     Push-Notifizierung für ein Gerät
    /// </summary>
    public class ExPushNotificationData
    {
        #region Properties

        /// <summary>
        ///     Geräte ID
        /// </summary>
        public int DeviceId { get; set; }

        /// <summary>
        ///     Titel
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Nachricht
        /// </summary>
        public string Body { get; set; }

        #endregion
    }
}