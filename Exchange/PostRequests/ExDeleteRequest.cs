// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.PostRequests
{
    /// <summary>
    ///     <para>Löschen</para>
    ///     Klasse ExDeleteRequest. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExDeleteRequest : IBissSerialize, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        ///     Eintrag zu löschen
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Passwortcheck Delete
        /// </summary>
        public string CheckPassword { get; set; }

        #endregion

#pragma warning disable 0067
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
    }
}