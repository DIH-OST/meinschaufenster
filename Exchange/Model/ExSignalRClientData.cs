// DigitalesSchaufenster (C) 2020 DIH-OST

namespace Exchange.Model
{
    /// <summary>
    ///     Clientdaten
    /// </summary>
    public class ExSignalRClientData
    {
        #region Properties

        /// <summary>
        ///     SignalRClient
        /// </summary>
        public ExSignalRClient Client { get; set; }

        /// <summary>
        ///     Schlüssel
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     Wert
        /// </summary>
        public string Value { get; set; }

        #endregion
    }
}