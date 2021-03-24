// DigitalesSchaufenster (C) 2020 DIH-OST

namespace Database.Tables
{
    /// <summary>
    ///     <para>Kreuztabelle Shop und Lieferoptionen</para>
    ///     Klasse TableStoreDelivery. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Table("StoreDelivery")]
    public class TableStoreDelivery
    {
        #region Properties

        /// <summary>
        ///     DB ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        ///     Shop Id
        /// </summary>
        [ForeignKey(nameof(TblStore))]
        public int TblStoreId { get; set; }

        /// <summary>
        ///     Shop
        /// </summary>
        public virtual TableStore TblStore { get; set; }

        /// <summary>
        ///     Liefermöglichkeit Id
        /// </summary>
        [ForeignKey(nameof(TblDeliveryOption))]
        public int TblDeliveryOptionId { get; set; }

        /// <summary>
        ///     Liefermöglichkeit
        /// </summary>
        public virtual TableDeliveryOption TblDeliveryOption { get; set; }

        #endregion
    }
}