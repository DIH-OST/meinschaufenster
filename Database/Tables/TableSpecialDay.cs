// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Database.Tables
{
    /// <summary>
    ///     <para>Tabelle für Sondertage</para>
    ///     Klasse TableSpecialDay. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Table("SpecialDay")]
    public class TableSpecialDay
    {
        #region Properties

        /// <summary>
        ///     DB ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        ///     Datum
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        ///     Begründung
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        ///     Geschäft welches den Sondertag hat
        /// </summary>
        public virtual TableStore Store { get; set; }

        /// <summary>
        ///     ID des Geschäfts welches den Sondertag hat
        /// </summary>
        [ForeignKey(nameof(Store))]
        public int StoreId { get; set; }

        #endregion
    }
}