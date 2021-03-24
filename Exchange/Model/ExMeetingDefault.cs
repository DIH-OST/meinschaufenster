// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Model
{
    /// <summary>
    ///     <para>PLatzhalter für Meeting</para>
    ///     Klasse ExMeetingDefault. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExMeetingDefault : IBissSerialize, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        ///     Platzhaltertext für optionalen Kommentar
        /// </summary>
        public string PlaceholderText { get; set; }

        /// <summary>
        ///     Freier Mitarbeiter für den Timeslot
        /// </summary>
        public int? FreeEmployeeId { get; set; }

        #endregion

#pragma warning disable 0067
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
    }
}