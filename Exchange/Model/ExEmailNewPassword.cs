// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Model
{
    /// <summary>
    ///     <para>EMail neues Passwort</para>
    ///     Klasse ExEmailNewPassword. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExEmailNewPassword : IBissSerialize, INotifyPropertyChanged
    {
        #region Properties

        public string Message { get; set; }

        public string NewPassword { get; set; }

        public string AlternativeText { get; set; }

        public string ApproveLink { get; set; }

        #endregion

#pragma warning disable 0067
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
    }
}