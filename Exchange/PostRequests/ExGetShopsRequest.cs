// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Biss.Apps.Components.Map.Base;

namespace Exchange.PostRequests
{
    /// <summary>
    ///     <para>Shops mit Filter Laden</para>
    ///     Klasse ExGetShopsRequest. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExGetShopsRequest : IBissSerialize, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        ///     Meine Position
        /// </summary>
        public BissPosition MyPosition { get; set; }

        /// <summary>
        ///     Gesetzte Range im UI
        /// </summary>
        public double Range { get; set; }

        /// <summary>
        ///     Filter Kategorie
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        ///     Filter Zahlmöglichkeit
        /// </summary>
        public int? PaymentMehodId { get; set; }

        /// <summary>
        ///     Filter Liefermöglichkeit
        /// </summary>
        public int? DeliveryMethodId { get; set; }

        #endregion

#pragma warning disable 0067
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
    }
}