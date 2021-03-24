// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Model
{
    /// <summary>
    ///     <para>Filterinfos</para>
    ///     Klasse ExFilterInfos. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExFilterInfos : IBissSerialize, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        ///     Kategorien
        /// </summary>
        public List<ExCategory> Categories { get; set; }

        /// <summary>
        ///     Zahlungsmöglichkeiten
        /// </summary>
        public List<ExPaymentMethod> PaymentMethods { get; set; }

        /// <summary>
        ///     Liefermöglichkeiten
        /// </summary>
        public List<ExDeliveryMethod> DeliveryMethods { get; set; }

        #endregion

#pragma warning disable 0067
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
    }
}