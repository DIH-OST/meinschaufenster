// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Model
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse ExRadListItem. (C) 2018 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExRadListItem : IBissSerialize, INotifyPropertyChanged
    {
        #region Properties

        public DateTime TimeStamp { get; set; }

        public int Value { get; set; }

        public int ElemNumber { get; set; }

        #endregion

#pragma warning disable 0067
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
    }
}