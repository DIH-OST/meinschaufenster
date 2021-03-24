// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.PostRequests
{
    /// <summary>
    ///     <para>Termin Speichern</para>
    ///     Klasse ExSaveMeetingRequest. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExSaveMeetingRequest : IBissSerialize, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        ///     Shop
        /// </summary>
        public int LocationId { get; set; }

        /// <summary>
        ///     User
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        ///     gewünschter Mitarbeiter
        /// </summary>
        public int? StaffId { get; set; }

        /// <summary>
        ///     Startzeit für Termin UTC
        /// </summary>
        [Obsolete]
        public DateTime StartTime { get; set; }

        /// <summary>
        ///     Datum
        /// </summary>
        public DateTime OnDate { get; set; }

        /// <summary>
        ///     Zeit im Format HH:mm
        /// </summary>
        public string OnTime { get; set; }

        /// <summary>
        ///     Optionaler Text für Termin
        /// </summary>
        public string OptionalText { get; set; }

        /// <summary>
        ///     Checkpasswort WebApp
        /// </summary>
        public string CheckPassword { get; set; }

        #endregion

#pragma warning disable 0067
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
    }
}