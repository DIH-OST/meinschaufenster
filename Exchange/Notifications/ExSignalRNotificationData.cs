// DigitalesSchaufenster (C) 2020 DIH-OST

namespace Exchange.Notifications
{
    /// <summary>
    ///     Nachrichteninhalt mit Benutzerinformationen
    /// </summary>
    public class ExSignalRNotificationData : ExNotificationData
    {
        #region Properties

        /// <summary>
        ///     Benutzer ID
        ///     Wenn die BenutzerID kleiner gleich 0, dann sind alle Benutzer betroffen
        /// </summary>
        [JsonProperty(PropertyName = "userid")]
        public long UserId { get; set; }

        #endregion
    }
}