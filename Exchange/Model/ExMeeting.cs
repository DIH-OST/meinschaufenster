// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Model
{
    /// <summary>
    ///     <para>Termin</para>
    ///     Klasse ExMeeting. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExMeeting : IBissSerialize, INotifyPropertyChanged, IComparable
    {
        #region Properties

        /// <summary>
        ///     DB Id wenn kleiner 0 - freier Termin
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Zeit Beginn wird Formatiert als "HH:mm"
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        ///     Dauer in Minuten
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        ///     Von UTC
        ///     TODO check UTC!
        /// </summary>
        [Obsolete]
        public DateTime Start { get; set; }

        /// <summary>
        ///     Bis UTC
        ///     TODO check UTC!
        /// </summary>
        [Obsolete]
        public DateTime End { get; set; }

        /// <summary>
        ///     Shop
        /// </summary>
        public int ShopId { get; set; }

        /// <summary>
        ///     Shop Name
        /// </summary>
        public string ShopName { get; set; }

        /// <summary>
        ///     Mitarbeiter
        /// </summary>
        public ExStaff Staff { get; set; }

        /// <summary>
        ///     User wenn null - freier Termin
        /// </summary>
        public int? UserId { get; set; }

        #endregion

#pragma warning disable 0067
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public int CompareTo(object obj)
        {
            return StartTime.CompareTo(obj);
        }
#pragma warning restore 0067
    }
}