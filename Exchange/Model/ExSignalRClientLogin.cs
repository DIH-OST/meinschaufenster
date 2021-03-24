// DigitalesSchaufenster (C) 2020 DIH-OST

namespace Exchange.Model
{
    /// <summary>
    ///     ClientLogin Daten
    /// </summary>
    public class ExSignalRClientLogin
    {
        #region Properties

        /// <summary>
        ///     SignalRClient
        /// </summary>
        public ExSignalRClient Client { get; set; }

        /// <summary>
        ///     BenutzerId
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        ///     Geräte-Token
        /// </summary>
        public string DeviceToken { get; set; }

        /// <summary>
        ///     UserToken
        /// </summary>
        public string UserToken { get; set; }

        #endregion
    }
}