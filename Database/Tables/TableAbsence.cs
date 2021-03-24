// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Database.Tables
{
    /// <summary>
    ///     <para>Tabelle für Abwesenheiten</para>
    ///     Klasse TableAbsence. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Table("Absence")]
    public class TableAbsence
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
        ///     Geschäft and dem es die Abwesenheiten gibt
        /// </summary>
        public virtual TableStore Store { get; set; }

        /// <summary>
        ///     Id des Geschäfts and dem es die Abwesenheiten gibt
        /// </summary>
        [ForeignKey(nameof(Store))]
        public int StoreId { get; set; }

        #endregion
    }
}