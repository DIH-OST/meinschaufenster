// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange.Enum;

namespace Exchange.PostRequests
{
    /// <summary>
    ///     <para>Termin löschen</para>
    ///     Klasse ExRemoveMeetingRequest. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExRemoveMeetingRequest : IBissSerialize, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        ///     User
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        ///     Termin zu löschen
        /// </summary>
        public int MeetingId { get; set; }

        /// <summary>
        ///     Wer löscht den Termin
        /// </summary>
        public EnumUserType UserType { get; set; } = EnumUserType.Customer;

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