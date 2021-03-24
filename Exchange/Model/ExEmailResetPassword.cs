// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Model
{
    /// <summary>
    ///     <para>Email Passwort Reset</para>
    ///     Klasse ExEmailResetPassword. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExEmailResetPassword : IBissSerialize, INotifyPropertyChanged
    {
        #region Properties

        public string Message { get; set; }

        public string ApproveLink { get; set; }

        public string AlternativeText { get; set; }

        #endregion

#pragma warning disable 0067
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
    }
}