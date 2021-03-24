// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.PostRequests
{
    /// <summary>
    ///     <para>Daten für das versenden einer Notifizierung</para>
    ///     Klasse ExPostNotofication. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExPostNotofication
    {
        #region Properties

        /// <summary>
        ///     Interner name für mobile.azure.com
        /// </summary>
        public string CampaignName { get; set; }

        /// <summary>
        ///     Inhalt der Nachricht
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        ///     Titel der Nachricht (zB App Name)
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Daten (optional)
        /// </summary>
        public Dictionary<string, string> Data { get; set; }

        #endregion
    }
}