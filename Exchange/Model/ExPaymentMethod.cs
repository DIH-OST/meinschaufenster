// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Model
{
    /// <summary>
    ///     <para>Bezahlmöglichkeit</para>
    ///     Klasse ExPaymentMethod. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExPaymentMethod : IBissSerialize, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        ///     DB Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Name für UI
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The unicode glyph used for the payment method.
        /// </summary>
        public string Glyph { get; set; }

        #endregion

#pragma warning disable 0067
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
    }
}