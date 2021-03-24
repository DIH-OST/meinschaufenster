// DigitalesSchaufenster (C) 2020 DIH-OST

namespace Database.Tables
{
    /// <summary>
    ///     <para>Tabelle für Liefermöglichkeiten</para>
    ///     Klasse TableDeliveryOption. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Table("DeliveryOption")]
    public class TableDeliveryOption
    {
        #region Properties

        /// <summary>
        ///     DB ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        ///     Bezeichnung
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Icon
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        ///     Zwischentabelle zwischen Geschäftt und Liefermöglichkeiten
        /// </summary>
        public virtual List<TableStoreDelivery> TblStoreDelivery { get; set; }

        #endregion
    }
}