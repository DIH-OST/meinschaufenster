// DigitalesSchaufenster (C) 2020 DIH-OST

namespace Database.Tables
{
    /// <summary>
    ///     <para>Kreuztabelle für Shop und Zahlungsmethoden</para>
    ///     Klasse TableStorePayment. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Table("StorePayment")]
    public class TableStorePayment
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
        ///     Zahlungsmöglichkeit Id
        /// </summary>
        [ForeignKey(nameof(TblPaymentOption))]
        public int TblPaymentOptionId { get; set; }

        /// <summary>
        ///     Zahlungsmöglichkeit
        /// </summary>
        public virtual TablePaymentOption TblPaymentOption { get; set; }

        #endregion
    }
}