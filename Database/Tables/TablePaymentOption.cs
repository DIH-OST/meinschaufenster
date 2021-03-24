// DigitalesSchaufenster (C) 2020 DIH-OST

namespace Database.Tables
{
    /// <summary>
    ///     <para>Tabelle für Bezahlmöglichkeiten</para>
    ///     Klasse TablePaymentOption. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Table("PaymentOption")]
    public class TablePaymentOption
    {
        #region Properties

        /// <summary>
        ///     db id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        ///     Beschreibung
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Icon
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        ///     Zwischentabelle für Geschäft und Bezahlmöglichkeit
        /// </summary>
        public virtual List<TableStorePayment> TblStorePayments { get; set; }

        #endregion
    }
}