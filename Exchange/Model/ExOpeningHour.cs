// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Model
{
    /// <summary>
    ///     <para>Öffnungzeiten</para>
    ///     Klasse ExOpeningHour. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExOpeningHour : IBissSerialize, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        ///     Tag
        /// </summary>
        public DateTime Day { get; set; }

        /// <summary>
        ///     Öffnet ab UTC - bei geschlossen null
        ///     TODO check UTC!
        /// </summary>
        public DateTime? TimeFrom { get; set; }

        /// <summary>
        ///     Schließt um UTC - bei geschlossen null
        ///     TODO check UTC!
        /// </summary>
        public DateTime? TimeTo { get; set; }

        /// <summary>
        ///     Speziell geöffneter Tag
        /// </summary>
        public bool IsSpecialDay { get; set; }

        /// <summary>
        ///     Speziell geschlossener Tag
        /// </summary>
        public bool IsAbscenceDay { get; set; }

        #endregion

#pragma warning disable 0067
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
    }
}