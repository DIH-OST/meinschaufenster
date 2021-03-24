// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Biss.Apps.Components.Map.Base;

namespace Exchange.Model
{
    /// <summary>
    ///     <para>Kurzinfos Shop</para>
    ///     Klasse ExShopShort. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExShopShort : IBissSerialize, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        ///     DB Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Firmenname
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     gerade geöffnet
        /// </summary>
        public bool IsOpen { get; set; }

        /// <summary>
        ///     Position für Karte
        /// </summary>
        public BissPosition Position { get; set; }

        /// <summary>
        ///     Hauptkategorie für Map Icon
        /// </summary>
        public ExCategory MainCategory { get; set; }

        /// <summary>
        ///     alle Kategorien des Shops
        /// </summary>
        public List<ExCategory> Categories { get; set; }

        /// <summary>
        ///     Unterstützte Bezahlmethoden
        /// </summary>
        public List<ExPaymentMethod> PaymentMethods { get; set; }

        /// <summary>
        ///     Unterstützte Liefermethoden
        /// </summary>
        public List<ExDeliveryMethod> DeliveryMethods { get; set; }

        #endregion

#pragma warning disable 0067
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
    }
}