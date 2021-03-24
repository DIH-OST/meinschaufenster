// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.PostRequests
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse ExGetMeetingsForDateRequest. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExGetMeetingsForDateRequest
    {
        #region Properties

        /// <summary>
        ///     Für Shop abfragen
        /// </summary>
        public int? LocationId { get; set; }

        /// <summary>
        ///     Für User Abfragen
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        ///     Datum
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        ///     bereits vergeben Termine auch ausgeben
        /// </summary>
        public bool ShowTakenSlots { get; set; }

        /// <summary>
        ///     Passwortcheck
        /// </summary>
        public string CheckPassword { get; set; }

        #endregion
    }
}