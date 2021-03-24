// DigitalesSchaufenster (C) 2020 DIH-OST

namespace WebRestApi.Models
{
    /// <summary>
    ///     <para>SignalRClient - SignalRClient </para>
    ///     Klasse SignalRClient (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class SignalRClient
    {
        #region Properties

        /// <summary>
        ///     Geräte-Token
        /// </summary>
        public string DeviceToken { get; set; }

        /// <summary>
        ///     Verbindugns-Id für SignalR
        /// </summary>
        public string ConnectionId { get; set; }

        /// <summary>
        ///     UserToken
        /// </summary>
        public string UserToken { get; set; }

        #endregion
    }
}