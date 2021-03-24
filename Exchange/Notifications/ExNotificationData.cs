// DigitalesSchaufenster (C) 2020 DIH-OST

namespace Exchange.Notifications
{
    /// <summary>
    ///     Nachrichteninhalt
    /// </summary>
    public class ExNotificationData
    {
        #region Properties

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
}