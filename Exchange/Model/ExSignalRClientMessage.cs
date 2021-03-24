// DigitalesSchaufenster (C) 2020 DIH-OST

using Exchange.Notifications;

namespace Exchange.Model
{
    /// <summary>
    ///     Nachricht an einen Client
    /// </summary>
    public class ExSignalRClientMessage : ExNotificationData
    {
        #region Properties

        /// <summary>
        ///     SignalRClient
        /// </summary>
        public ExSignalRClient Client { get; set; }

        #endregion
    }
}